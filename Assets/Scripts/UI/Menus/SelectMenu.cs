using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class SelectMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject menu;
    public GameObject cam;
    private camScript camScript;
    public GameObject menuButtonPrefab;
    private List<GameObject> buttons;

    private bool menuOpen;
    private bool frameDelay;

    void Start()
    {
        camScript = cam.GetComponent<camScript>();
        buttons = new List<GameObject>();
        DeactivateMenu();
    }

    void Update()
    {
        // TODO: put this in controller somehow? Allow for immediate right clicking on new thing while first menu is open?
        RaycastHit hit;
        if (frameDelay && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, -1, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject(-1))
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                DeactivateMenu();
            }
        }
        if(menuOpen) frameDelay = true;
    }

    public void DeactivateMenu()
    {
        Debug.Log("Deactivate menu");
        menu.SetActive(false);
        menuOpen = false;
        frameDelay = false;
        cam.GetComponent<camScript>().CamHalt(false); // TODO: this should be handled by UI manager
        foreach (GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons = new List<GameObject>();
    }

    public void ActivateMenu(Vector3 pos, List<(string, Action)> actionList)
    {
        menu.SetActive(true);
        menuOpen = true;
        cam.GetComponent<camScript>().CamHalt(true);
        int i = 0;
        Debug.Log("action list: ");
        foreach ((string, Action) entry in actionList) {  // TODO: possibly make list of tuples to preserve order
            Debug.Log(entry.Item1);
            buttons.Add(Instantiate(menuButtonPrefab, gameObject.transform));
            buttons[i].GetComponent<ButtonScript>().buttonText.text = entry.Item1;
            buttons[i].GetComponent<Button>().onClick.AddListener(() => entry.Item2.Invoke());
            buttons[i].GetComponent<Button>().onClick.AddListener(() => DeactivateMenu());
            buttons[i].transform.SetParent(menu.transform);
            i++;
        }
        var menuDims = menu.GetComponent<RectTransform>().rect;
        Debug.Log("pos: " + pos);
        Debug.Log("dims: " + menuDims);
        var padding = menu.GetComponent<VerticalLayoutGroup>().padding;
        var spacing = menu.GetComponent<VerticalLayoutGroup>().spacing;
        var buttonDims = menuButtonPrefab.GetComponent<RectTransform>().rect;
        var height = padding.top + (actionList.Count * buttonDims.height) + padding.bottom;
        var width = padding.left + buttonDims.width + padding.right;
        menu.transform.position = new Vector3(pos.x + width/2, pos.y - height/2, 0);
    }
}

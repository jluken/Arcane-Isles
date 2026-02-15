using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSelectMenu : MenuScreen
{
    public static ItemSelectMenu Instance { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject menu;
    private GameObject menuButtonPrefab;
    private List<GameObject> buttons;

    private bool menuOpen;
    private bool frameDelay;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        menuButtonPrefab = Resources.Load<GameObject>("Prefabs/MenuButton");
        buttons = new List<GameObject>();
    }

    void Update()
    {
        RaycastHit hit;
        if (frameDelay && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, -1, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject(-1))
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                UIController.Instance.CloseOverlays();
            }
        }
        if(menuOpen) frameDelay = true;
    }

    public override bool IsActive()
    {
        return menuOpen;
    }

    public override bool overlay => true;

    public override void DeactivateMenu()
    {
        Debug.Log("Deactivate menu");
        menu.SetActive(false);
        menuOpen = false;
        frameDelay = false;
        foreach (GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons = new List<GameObject>();
    }

    public void SetItemSelection(Vector3 pos, List<SelectionData> actionList)
    {
        int i = 0;
        Debug.Log("action list: ");
        foreach (SelectionData entry in actionList)
        {  // TODO: possibly make list of tuples to preserve order
            buttons.Add(Instantiate(menuButtonPrefab, gameObject.transform));
            buttons[i].GetComponent<ButtonScript>().buttonText.text = entry.actionName;
            buttons[i].GetComponent<Button>().onClick.AddListener(() => SelectionController.Instance.InitiateSelection(entry));
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
        menu.transform.position = new Vector3(pos.x + width / 2, pos.y - height / 2, 0);
    }

    public override void ActivateMenu()
    {
        menu.SetActive(true);
        menuOpen = true;
    }
}

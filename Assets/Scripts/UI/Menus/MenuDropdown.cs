using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class MenuDropdown : MenuScreen
{
    public static MenuDropdown Instance { get; private set; }

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

        InputActionMap uiActions = InputSystem.actions.FindActionMap("UI");
        uiActions.FindAction("Click").performed += (sender) => HandleClick();
        uiActions.FindAction("RightClick").performed += (sender) => HandleClick();
    }

    void Update()
    {
        if (menuOpen) frameDelay = true;
    }

    private void HandleClick()
    {
        if (frameDelay && EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject())
        {
            DeactivateMenu();
        }
    }

    public override bool IsActive() => menuOpen;

    public override void DeactivateMenu()
    {
        if (!IsActive()) return;
        menu.SetActive(false);
        menuOpen = false;
        frameDelay = false;
        foreach (GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons = new List<GameObject>();
    }

    public void SetItemSelection(Vector3 pos, Vector3 itemPos, ItemSlot itemSlot)
    {
        //int i = 0;

        var dropButton = Instantiate(menuButtonPrefab, gameObject.transform);
        dropButton.GetComponent<ButtonScript>().buttonText.text = "drop item";
        dropButton.GetComponent<Button>().onClick.AddListener(() => itemSlot.DropStack(itemPos));
        dropButton.GetComponent<Button>().onClick.AddListener(() => DeactivateMenu());
        dropButton.transform.SetParent(menu.transform);
        buttons.Add(dropButton);
 
        var splitButton = Instantiate(menuButtonPrefab, gameObject.transform);
        splitButton.GetComponent<ButtonScript>().buttonText.text = "split 1 item";
        splitButton.GetComponent<Button>().onClick.AddListener(() => itemSlot.SplitOff(1));
        splitButton.GetComponent<Button>().onClick.AddListener(() => DeactivateMenu());
        splitButton.transform.SetParent(menu.transform);
        buttons.Add(splitButton);

        var splitHalfButton = Instantiate(menuButtonPrefab, gameObject.transform);
        splitHalfButton.GetComponent<ButtonScript>().buttonText.text = "split half items";
        splitHalfButton.GetComponent<Button>().onClick.AddListener(() => itemSlot.SplitOff(itemSlot.currentStack / 2));
        splitHalfButton.GetComponent<Button>().onClick.AddListener(() => DeactivateMenu());
        splitHalfButton.transform.SetParent(menu.transform);
        buttons.Add(splitHalfButton);

        var menuDims = menu.GetComponent<RectTransform>().rect;
        var padding = menu.GetComponent<VerticalLayoutGroup>().padding;
        var spacing = menu.GetComponent<VerticalLayoutGroup>().spacing;
        var buttonDims = menuButtonPrefab.GetComponent<RectTransform>().rect;
        var height = padding.top + (buttons.Count * buttonDims.height) + padding.bottom;
        var width = padding.left + buttonDims.width + padding.right;
        menu.transform.position = new Vector3(pos.x + width / 2, pos.y - height / 2, 0);
    }

    public override void ActivateMenu()
    {
        menu.SetActive(true);
        menuOpen = true;
    }
}

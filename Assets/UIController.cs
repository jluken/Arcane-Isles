using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    //private camScript camScript;

    //public GameObject map;
    public MenuScreen mapScript;
    //public GameObject buttons;
    public MenuScreen buttonsScript;
    //public GameObject dialog;
    public DialogueBox dialogScript;
    //public GameObject journal;
    public MenuScreen journalScript;
    //public GameObject pauseScreen;
    public MenuScreen pauseScreenScript;
    //public GameObject charScreen;
    public PlayerInventoryMenu charScreenScript;
    //public GameObject tradeScreen;
    public TradingMenu tradeScreenScript;
    public ContainerInventoryMenu containerScreenScript;
    public ItemSelectMenu itemSelectScript;

    private List<MenuScreen> screens;
    //private List<MenuScreen> exitableScreens;
    //private List<MenuScreen> stickyScreens;

    private Dictionary<KeyCode, MenuScreen> screenKeyCodes;

    private void Awake()
    {
        Instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //camScript = camScript.Instance;
        screens = new List<MenuScreen>();
        //mapScript = map.GetComponent<MenuScreen>();
        //buttonsScript = buttons.GetComponent<MenuScreen>();
        //dialogScript = dialog.GetComponent<DialogueBox>();
        //journalScript = journal.GetComponent<MenuScreen>();
        //pauseScreenScript = pauseScreen.GetComponent<MenuScreen>();
        //charScreenScript = charScreen.GetComponent<InventoryManager>();
        //tradeScreenScript = tradeScreen.GetComponent<InventoryManager>();
        screens = new List<MenuScreen> { mapScript, buttonsScript, dialogScript, journalScript, pauseScreenScript, charScreenScript, containerScreenScript, tradeScreenScript, itemSelectScript };
        //exitableScreens = new List<MenuScreen> { mapScript, journalScript, pauseScreenScript, charScreenScript, tradeScreenScript };
        //stickyScreens = new List<MenuScreen> { buttonsScript, dialogScript };

        screenKeyCodes = new Dictionary<KeyCode, MenuScreen>
        {
            { KeyCode.M, mapScript },
            { KeyCode.J, journalScript },
            { KeyCode.I , charScreenScript},
            { KeyCode.C , charScreenScript}
        };

        ActivateDefaultScreen();
    }

    void Update()
    {
        foreach (KeyCode screenKey in screenKeyCodes.Keys)
        {
            if(Input.GetKeyDown(screenKey) && !screenKeyCodes[screenKey].IsActive())
            {
                CloseOverlays();
                Time.timeScale = 0; // Pause
                screenKeyCodes[screenKey].ActivateMenu();
            }
            else if (screenKeyCodes[screenKey].IsActive() && Input.GetKeyDown(screenKey))
            {
                //screenKeyCodes[screenKey].DeactivateMenu();
                CloseOverlays();
                Time.timeScale = 1; // Unpause
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (OverlayOpen()) CloseOverlays();
            else ActivatePauseScreen();
        }
    }

    void DeactivateAllMenus()
    {
        foreach (MenuScreen screen in screens)
        {
            if (screen.IsActive()) screen.DeactivateMenu();
        }
    }

    public void ActivateDefaultScreen()
    {
        DeactivateAllMenus();
        Time.timeScale = 1; // Unpause
        buttonsScript.ActivateMenu();
        //TODO: Activate AviScreenScript separately (will still stay up if buttons go away for dialogue screen)
    }

    public void CloseOverlays()
    {
        foreach (MenuScreen screen in screens)
        {
            if (screen.overlay && screen.IsActive()) screen.DeactivateMenu();
        }
        Time.timeScale = 1; // Unpause
    }

    public bool OverlayOpen()
    {
        return screens.Where(s => s.overlay && s.IsActive()).Any();
    }

    public void ActivateMapScreen()
    {
        DeactivateAllMenus();
        Time.timeScale = 0; // Pause
        mapScript.ActivateMenu();
    }

    public void ActivateJournalScreen()
    {
        DeactivateAllMenus();
        Time.timeScale = 0; // Pause
        journalScript.ActivateMenu();
    }

    public void ActivateDialog(List<string> newDialogue, Sprite speakerPic)
    {
        DeactivateAllMenus();
        //TODO: Activate AviScreenScript separately (will still stay up if buttons go away for dialogue screen)
        Time.timeScale = 0; // Pause
        dialogScript.SetSpeaker(newDialogue, speakerPic);
        dialogScript.ActivateMenu();
    }

    public void ActivatePauseScreen()
    {
        Time.timeScale = 0; // Pause
        pauseScreenScript.ActivateMenu();
    }

    public InventoryMenu CurrentInventory()
    {
        if (charScreenScript.IsActive()) return charScreenScript;
        else if (containerScreenScript.IsActive()) return containerScreenScript;
        else if (tradeScreenScript.IsActive()) return tradeScreenScript;
        else return null;
    }

    public void ActivateCharScreen()
    {
        DeactivateAllMenus();
        Time.timeScale = 0; // Pause
        charScreenScript.ActivateMenu();
    }

    public void ActivateTradeScreen(EntityInventory inventory)
    {
        DeactivateAllMenus();
        Time.timeScale = 0; // Pause
        tradeScreenScript.SetInventory(inventory);
        tradeScreenScript.ActivateMenu();
    }

    public void ActivateContainerScreen(EntityInventory inventory)
    {
        DeactivateAllMenus();
        Time.timeScale = 0; // Pause
        containerScreenScript.SetInventory(inventory);
        containerScreenScript.ActivateMenu();
    }

    public void ActivateItemSelect(Vector3 pos, List<(string, Action)> actionList)
    {
        //DeactivateAllMenus();
        //Time.timeScale = 0; // Pause
        itemSelectScript.SetItemSelection(pos, actionList);
        itemSelectScript.ActivateMenu();
    }

    public bool DefaultUIOpen()
    {
        return buttonsScript.IsActive() && !OverlayOpen();
    }
}

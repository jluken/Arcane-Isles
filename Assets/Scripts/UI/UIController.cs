using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.Device;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    //private camScript camScript;

    //public GameObject map;
    public MenuScreen mapScript;  //TODO: if all these are instances, any need to declare them?
    //public GameObject buttons;
    public DefaultUI defaultUI;
    //public GameObject dialog;
    public DialogueBox dialogScript;
    //public GameObject journal;
    public MenuScreen journalScript;
    //public GameObject pauseScreen;
    public MenuScreen pauseScreenScript;
    //public GameObject charScreen;
    public PlayerInventoryMenu charInventoryScript;
    //public GameObject tradeScreen;
    public TradingMenu tradeScreenScript;
    public ContainerInventoryMenu containerScreenScript;
    public ItemSelectMenu itemSelectScript;
    public SettingsMenu settingsMenu;
    //public MainMenu mainMenu;
    public CharacterMenu characterMenu;

    public GameOverScreen gameOverScreen;

    private List<MenuScreen> screens;
    //private List<MenuScreen> exitableScreens;
    //private List<MenuScreen> stickyScreens;

    private Dictionary<KeyCode, MenuScreen> screenKeyCodes;

    private bool menusAllowed;

    private void Awake()
    {
        Instance = this;
        // TODO: break this list up into list types
        screens = new List<MenuScreen> { mapScript, defaultUI, dialogScript, journalScript, pauseScreenScript, charInventoryScript, containerScreenScript, tradeScreenScript, itemSelectScript, settingsMenu, characterMenu, gameOverScreen };// mainMenu };
        menusAllowed = false;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        screenKeyCodes = new Dictionary<KeyCode, MenuScreen>
        {
            { KeyCode.M, mapScript },
            { KeyCode.J, journalScript },
            { KeyCode.I , charInventoryScript},
            { KeyCode.C , characterMenu}
        };

        //ActivateDefaultScreen();
        DeactivateAllMenus();
    }

    public void AllowMenus(bool allow)
    {
        menusAllowed = allow;  //TODO: handle this more about whether "in game" behavior vs main menu
    }

    void Update()  // TODO: possibly make UI a state machine?
    {
        //TODO: limit menus you can open if not playerUnderControl
        if (!menusAllowed) return;
        foreach (KeyCode screenKey in screenKeyCodes.Keys)
        {
            if(Input.GetKeyDown(screenKey) && !screenKeyCodes[screenKey].IsActive())
            {
                CloseOverlays();
                Time.timeScale = 0; // Pause
                Debug.Log("Pause Time");
                screenKeyCodes[screenKey].ActivateMenu();
            }
            else if (screenKeyCodes[screenKey].IsActive() && Input.GetKeyDown(screenKey))
            {
                Debug.Log("Close open screen");
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

    // Update is called once per frame
    void LateUpdate()
    {
        
    }

    public void DeactivateAllMenus()
    {
        foreach (MenuScreen screen in screens)
        {
            if (screen.IsActive()) screen.DeactivateMenu();
        }
    }

    public void ActivateDefaultScreen()
    {
        DeactivateAllMenus();
        //Debug.Log("Default");
        Time.timeScale = 1; // Unpause
        defaultUI.ActivateMenu();
        //TODO: Activate AviScreenScript separately (will still stay up if buttons go away for dialogue screen)
    }

    public void ActivateGameOver()
    {
        DeactivateAllMenus();
        Time.timeScale = 0;
        Debug.Log("Pause Time");
        gameOverScreen.ActivateMenu();
    }

    public void ActivateCombatUI()
    {
        DeactivateAllMenus();
        Time.timeScale = 1; // Unpause
        defaultUI.ActivateCombat();
        //TODO: Activate AviScreenScript separately (will still stay up if buttons go away for dialogue screen)
    }

    public void CloseOverlays()
    {
        foreach (MenuScreen screen in screens)
        {
            if (screen.overlay && screen.IsActive()) screen.DeactivateMenu();
        }
        if (!screens.Any(scr => scr.IsActive())) ActivateDefaultScreen(); // TODO: this is a hack that should be handled better by manager by specifying which overlays "turn off" default screen
        Time.timeScale = 1; // Unpause
    }

    public bool OverlayOpen()  // TODO: make overlays still block whole screen from clicking, and section off where pausing/unpausing happens
    {
        return screens.Where(s => s.overlay && s.IsActive()).Any();
    }

    public void ActivateMapScreen()
    {
        DeactivateAllMenus();
        Time.timeScale = 0; // Pause
        Debug.Log("Pause Time");
        mapScript.ActivateMenu();
    }

    public void ActivateJournalScreen()
    {
        DeactivateAllMenus();
        Time.timeScale = 0; // Pause
        Debug.Log("Pause Time");
        journalScript.ActivateMenu();
    }

    public void ActivateDialog(List<string> newDialogue, Sprite speakerPic)
    {
        DeactivateAllMenus();
        //TODO: Activate AviScreenScript separately (will still stay up if buttons go away for dialogue screen)
        Time.timeScale = 0; // Pause
        Debug.Log("Pause Time");
        dialogScript.SetSpeaker(newDialogue, speakerPic);
        dialogScript.ActivateMenu();
    }

    public void ActivatePauseScreen()
    {
        Time.timeScale = 0; // Pause
        Debug.Log("Pause Time");
        pauseScreenScript.ActivateMenu();
    }

    public InventoryMenu CurrentInventory()
    {
        if (charInventoryScript.IsActive()) return charInventoryScript;
        else if (containerScreenScript.IsActive()) return containerScreenScript;
        else if (tradeScreenScript.IsActive()) return tradeScreenScript;
        else return null;
    }

    public void ActivateCharInventoryScreen()
    {
        DeactivateAllMenus();
        Time.timeScale = 0; // Pause
        Debug.Log("Pause Time");
        charInventoryScript.ActivateMenu();
    }

    public void ActivateCharScreen()
    {
        DeactivateAllMenus();
        Time.timeScale = 0; // Pause
        Debug.Log("Pause Time");
        characterMenu.ActivateMenu();
    }

    public void ActivateTradeScreen(EntityInventory inventory)
    {
        DeactivateAllMenus();
        Time.timeScale = 0; // Pause
        Debug.Log("Pause Time");
        tradeScreenScript.SetInventory(inventory);
        tradeScreenScript.ActivateMenu();
    }

    public void ActivateContainerScreen(EntityInventory inventory)
    {
        DeactivateAllMenus();
        Time.timeScale = 0; // Pause
        Debug.Log("Pause Time");
        containerScreenScript.SetInventory(inventory);
        containerScreenScript.ActivateMenu();
    }

    public void ActivateItemSelect(Vector3 pos, List<SelectionData> actionList)
    {
        //DeactivateAllMenus();
        //Time.timeScale = 0; // Pause
        itemSelectScript.SetItemSelection(pos, actionList);
        itemSelectScript.ActivateMenu();
    }

    public bool DefaultUIOpen()
    {
        return defaultUI.IsActive() && !OverlayOpen();
    }

    public void ActivateSettings()
    {
        DeactivateAllMenus();
        Time.timeScale = 0; // Pause
        Debug.Log("Pause Time");
        settingsMenu.ActivateMenu();
    }

    //public void ActivateMainMenu()
    //{
    //    DeactivateAllMenus();
    //    Time.timeScale = 0; // Pause
    //    mainMenu.ActivateMenu();
    //}
}

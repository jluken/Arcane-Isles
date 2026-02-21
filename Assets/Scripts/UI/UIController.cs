using PixelCrushers.DialogueSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.Device;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    // static objetc declaration required to ensure existence before access in Start scripts

    //Scene pages - restricted interation and no buttons allowed. Time paused
    public MainMenu mainMenu;
    public LoadingScreen loadingScreen;
    public CharCreateMenu charCreateMenu;
    public GameOverScreen gameOverScreen;

    // Default gameplay UI
    public DefaultUI defaultUI;

    //Journal menus - accessible during normal gameplay through key/button press. Time Paused
    public MapScript mapScript;
    public JournalScript journalScript;
    public PlayerInventoryMenu charInventoryMenu;
    public CharacterMenu characterMenu;

    // Special menus which override 
    public PauseMenuScript pauseScreenScript;
    public SettingsMenu settingsMenu;

    //Interaction menus - accessed by specific interactions
    public ContainerInventoryMenu containerScreenScript;
    public TradingMenu tradeScreenScript;

    // Special in-frame menu that does not pause - sticks around until custom condition
    public ItemSelectMenu itemSelectMenu;

    private List<MenuScreen> SceneScreens;
    private List<MenuScreen> LogbookMenus;
    private List<MenuScreen> PauseMenus;
    private List<MenuScreen> InteractionMenus;

    //Groupings
    private List<MenuScreen> OverlayMenus;
    private List<MenuScreen> JournalLockMenus;

    private List<MenuScreen> AllMenus;

    private InputActionMap uiActions;
    private Dictionary<string, MenuScreen> screenKeyCodes;

    private bool talking;

    private void Awake()
    {
        Instance = this;

        SceneScreens = new List<MenuScreen>() {
            mainMenu,
            loadingScreen,
            charCreateMenu,
            gameOverScreen
        };

        LogbookMenus = new List<MenuScreen>()
        {
            mapScript,
            journalScript,
            charInventoryMenu,
            characterMenu
        };

        PauseMenus = new List<MenuScreen>() {
            pauseScreenScript,
            settingsMenu
        };

        InteractionMenus = new List<MenuScreen>()
        {
            containerScreenScript,
            tradeScreenScript
        };

        JournalLockMenus = new List<MenuScreen>();
        JournalLockMenus.AddRange(SceneScreens);
        JournalLockMenus.AddRange(PauseMenus);
        JournalLockMenus.AddRange(InteractionMenus);


        OverlayMenus = new List<MenuScreen>();
        OverlayMenus.AddRange(PauseMenus);
        OverlayMenus.AddRange(InteractionMenus);
        OverlayMenus.AddRange(LogbookMenus);
        OverlayMenus.Add(itemSelectMenu);

        AllMenus = new List<MenuScreen>();
        AllMenus.AddRange(SceneScreens);
        AllMenus.AddRange(LogbookMenus);
        AllMenus.AddRange(PauseMenus);
        AllMenus.AddRange(InteractionMenus);
        AllMenus.Add(defaultUI);
        AllMenus.Add(itemSelectMenu);

        uiActions = InputSystem.actions.FindActionMap("UI");
        screenKeyCodes = new Dictionary<string, MenuScreen>
        {
            { "Map", mapScript },
            { "Journal", journalScript },
            { "Inventory" , charInventoryMenu},
            { "CharScreen" , characterMenu}
        };
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DialogueManager.instance.conversationStarted += (sender) => ToggleConversationUI(true);
        DialogueManager.instance.conversationEnded += (sender) => ToggleConversationUI(false);

        foreach(string key in screenKeyCodes.Keys) uiActions.FindAction(key).performed += (sender) => HandleJournalKey(key);
        uiActions.FindAction("Cancel").performed += (sender) => HandleCancel();
    }

    public bool PauseTime()
    {
        var pausedMenus = new List<MenuScreen>();
        pausedMenus.AddRange(LogbookMenus);
        pausedMenus.AddRange(PauseMenus);
        pausedMenus.AddRange(InteractionMenus);
        return pausedMenus.Any(menu => menu.IsActive()) || talking;
    }

    void Update()
    {
        Time.timeScale = PauseTime() ? 0 : 1;
    }

    private void HandleCancel()
    {
        bool noButtonMenus = SceneScreens.Any(menu => menu.IsActive()) || talking;
        if (OverlayMenus.Any(menu => menu.IsActive())) CloseOverlays();
        else if (!noButtonMenus) pauseScreenScript.ActivateMenu();
    }
    private void HandleJournalKey(string key)
    {
        bool noButtonMenus = SceneScreens.Any(menu => menu.IsActive()) || talking;
        if (!noButtonMenus && !JournalLockMenus.Any(menu => menu.IsActive()))
        {
            var currOpen = screenKeyCodes[key].IsActive();
            CloseLogbook();
            if (!currOpen)
            {
                screenKeyCodes[key].ActivateMenu();
            }
        }
    }

    public void CloseLogbook()
    {
        foreach (var menu in LogbookMenus) { menu.DeactivateMenu(); }
    }

    public void CloseOverlays()
    {
        foreach (var menu in OverlayMenus) { menu.DeactivateMenu(); }
    }

    public void DeactivateAllMenus()
    {
        foreach (var menu in AllMenus) { menu.DeactivateMenu(); }
    }

    public void ActivateDefaultScreen()
    {
        DeactivateAllMenus();
        defaultUI.ActivateMenu();
    }

    public void ActivateGameOver()
    {
        DeactivateAllMenus();
        gameOverScreen.ActivateMenu();
    }

    public void ActivateCombatUI()
    {
        DeactivateAllMenus();
        defaultUI.ActivateCombat();
    }

    public void ActivateTradeScreen(EntityInventory inventory)
    {
        CloseOverlays();
        tradeScreenScript.SetInventory(inventory);
        tradeScreenScript.ActivateMenu();
    }

    public void ActivateContainerScreen(EntityInventory inventory)
    {
        CloseOverlays();
        containerScreenScript.SetInventory(inventory);
        containerScreenScript.ActivateMenu();
    }

    public void ActivateItemSelect(Vector3 pos, List<SelectionData> actionList)
    {
        itemSelectMenu.SetItemSelection(pos, actionList);
        itemSelectMenu.ActivateMenu();
    }

    public void ActivateSettings()
    {
        CloseOverlays();
        settingsMenu.ActivateMenu();
    }

    public void ActivateLoadingScreen()
    {
        DeactivateAllMenus();
        loadingScreen.ActivateMenu();
    }

    public void ActivateMainMenu()
    {
        DeactivateAllMenus();
        mainMenu.ActivateMenu();
    }

    public void ToggleConversationUI(bool setTalking)
    {
        talking = setTalking;

    }
}

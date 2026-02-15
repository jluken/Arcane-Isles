using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
//ing UnityEngine.UIElements;

public class DefaultUI : MenuScreen 
{
    public static DefaultUI Instance { get; private set; }

    //[Serializable]
    //public class CharIconData
    //{
    //    public int charSlot;
    //    public GameObject charPortrait;
    //    public Image healthBarBackground;
    //    public Image healthBar;
    //    public TMP_Text Health;
    //}

    public GameObject UIBar;

    public GameObject ActionMenu;
    public Button NextTurnButton;
    public GameObject ActionButtonPrefab;

    public GameObject CharIconPrefab;
    public GameObject initiativeBar;
    private List<GameObject> initiativeIcons;

    private List<GameObject> actionButtons;

    public GameObject ActionPointBar;
    private List<GameObject> ActionPointPips;
    public GameObject pipPrefab;

    public GameObject chatWindowScroll;
    public GameObject chatWindowContent;

    public charIcon[] charIcons = new charIcon[PartyController.maxParty];  // TODO: put these on a panel

    //public GameObject ButtonMenu;
    //public GameObject TextMenu;
    public bool UIActive;

    //private CharStats charStats;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start UI");
        //charStats = player.GetComponent<CharStats>();
        //charStats.updateStatEvent += UpdateStats;
        actionButtons = new List<GameObject>();
        initiativeIcons = new List<GameObject>();
        ActionPointPips = new List<GameObject>();

        PartyController.Instance.updatePartyEvent += UpdateStats;
        DialogueInterface.Instance.updateChatLog += UpdateChatUI;
    }

    public void SetScrollToBottom()
    {
        chatWindowScroll.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
    }

    public void UpdateStats() 
    {
        //Debug.Log("Update stat UI");
        var currentParty = PartyController.Instance.party;
        if (!UIActive) return; // TODO: possibly handle this better by separating out the "default" UI

        for (int i = 0; i < charIcons.Length; i++) { 
            charIcon charIcon = charIcons[i];
            if (i + 1 > currentParty.Count) charIcon.gameObject.SetActive(false);
            else
            {
                charIcon.gameObject.SetActive(true);
                charIcon.UpdateIcon(currentParty[i], true);
            }
        }
        UpdateActions(PartyController.Instance.leader);
    }

    public void ActivateCombat()
    {
        ActivateMenu();
        NextTurnButton.gameObject.SetActive(true);
        initiativeBar.SetActive(true);
        foreach (GameObject icon in initiativeIcons) Destroy(icon);
        initiativeIcons.Clear();
    }

    public void UpdateActions(NPC activeNPC)
    {
        foreach (GameObject but in actionButtons) Destroy(but);
        actionButtons.Clear();
        if (!CombatManager.Instance.IsPartyTurn)
        {
            ActionMenu.SetActive(false);
            NextTurnButton.enabled = false;
        }
        else
        {
            var actions = activeNPC.GetActions();
            ActionMenu.SetActive(true);
            NextTurnButton.enabled = CombatManager.Instance.combatActive;
            Debug.Log("Set actions: " + actions.Count);
            foreach (AbilityAction action in actions)
            {
                actionButtons.Add(Instantiate(ActionButtonPrefab, ActionMenu.transform));
                actionButtons.LastOrDefault().GetComponent<Button>().image.sprite = action.icon;
                actionButtons.LastOrDefault().GetComponent<Button>().onClick.AddListener(() => CombatManager.Instance.SetCurrentAction(action));
            }
        }
        FillActionPoints(activeNPC);
    }

    public void UpdateChatUI(List<string> latestChatLog)
    {
        chatWindowContent.GetComponent<TextMeshProUGUI>().text = "";
        foreach (var logEntry in latestChatLog)
        {
            chatWindowContent.GetComponent<TextMeshProUGUI>().text += logEntry + "\n";
        }
    }

    public void ListInitiatives(List<NPC> npcs)
    {
        foreach (GameObject icon in initiativeIcons) Destroy(icon);
        initiativeIcons.Clear();
        foreach (NPC npc in npcs)
        {
            var icon = Instantiate(CharIconPrefab, initiativeBar.transform);
            initiativeIcons.Add(icon);
            icon.GetComponent<charIcon>().UpdateIcon(npc);
        }

    }

    public void FillActionPoints(NPC activeNPC)
    {
        foreach (GameObject pip in ActionPointPips) Destroy(pip);
        ActionPointPips.Clear();
        var currentAP = CombatManager.Instance.GetCurrentAP(activeNPC);
        for(int i = 0; i < activeNPC.charStats.GetCurrStat(CharStats.StatVal.actionPoints); i++)
        {
            ActionPointPips.Add(Instantiate(pipPrefab, ActionPointBar.transform));
            //Debug.Log(ActionPointPips.LastOrDefault());
            //Debug.Log(ActionPointPips.LastOrDefault().GetComponent<Image>());
            //Debug.Log(ActionPointPips.LastOrDefault().GetComponent<Image>().sprite);
            if (i < currentAP) ActionPointPips.LastOrDefault().GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/fullPip"); // TODO: possibly handle differently
        }
    }

    public void NextTurn()
    {
        CombatManager.Instance.NextTurn();
    }

    public override void DeactivateMenu()
    {
        foreach (var charIcon in charIcons) { charIcon.gameObject.SetActive(false); }
        UIBar.SetActive(false);
        initiativeBar.SetActive(false);
        //TextMenu.SetActive(false);
        UIActive = false;
    }

    public override void ActivateMenu()
    {
        //Debug.Log("Activate Default");
        UIActive = true;
        UpdateStats();
        UIBar.SetActive(true);
        NextTurnButton.gameObject.SetActive(false);
        initiativeBar.SetActive(false);
        //TextMenu.SetActive(true);
    }

    public override bool IsActive()
    {
        return UIActive;
    }

    public override bool overlay => false;
}

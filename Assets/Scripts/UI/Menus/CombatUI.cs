using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
//ing UnityEngine.UIElements;

public class CombatUI : MenuScreen
{
    public static CombatUI Instance { get; private set; }

    public GameObject CombatMenu;
    public GameObject ActionMenu;
    public Button NextTurnButton;
    public GameObject ActionButtonPrefab;
    //public GameObject TextMenu;
    public bool UIActive;

    private List<GameObject> actionButtons;

    //private CharStats charStats;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actionButtons = new List<GameObject>();
        //PartyController.Instance.updatePartyEvent += UpdateStats;
    }

    public override void DeactivateMenu()
    {
        //foreach (var charIcon in charIcons) { charIcon.charPortrait.SetActive(false); }
        CombatMenu.SetActive(false);
        //TextMenu.SetActive(false);
        UIActive = false;
    }

    public override void ActivateMenu()
    {
        UIActive = true;
        //UpdateStats();
        CombatMenu.SetActive(true);
        //TextMenu.SetActive(true);
    }

    public override bool IsActive()
    {
        return UIActive;
    }

    public void UpdateActions(List<AbilityAction> actions, bool enemyTurn)
    {
        foreach (GameObject but in actionButtons) Destroy(but);
        actionButtons.Clear();
        if (enemyTurn)
        {
            ActionMenu.SetActive(false);
            NextTurnButton.enabled = false;
        }
        else
        {
            ActionMenu.SetActive(true);
            NextTurnButton.enabled = true;
            foreach (AbilityAction action in actions)
            {
                actionButtons.Add(Instantiate(ActionButtonPrefab, ActionMenu.transform));
                actionButtons.LastOrDefault().GetComponent<Button>().image.sprite = action.icon;
                actionButtons.LastOrDefault().GetComponent<Button>().onClick.AddListener(() => CombatManager.Instance.SetCurrentAction(action));
            }
        }        
    }

    public void NextTurn()
    {
        CombatManager.Instance.NextTurn();
    }
}

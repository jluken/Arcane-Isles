using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }  // TODO: include this stuff in save data

    public bool combatActive { get; private set; } = false ;

    public event Action callToArms;

    public struct InitiativeEntry
    {
        public NPC npc;
        public int initiative;
        public CombatantType type;
    }

    public enum CombatActionType
    {
        Run,
        Attack
    }

    public enum CombatantType
    {
        Party,
        Ally,
        Enemy,
        Bystander
    }

    public List<InitiativeEntry> combatantInitiative;

    public NPC activeCombatant;
    public AbilityAction currentAction;
    private int initiativeTurn;
    public int ActionPoints { get; private set; }

    public AbilityAction defaultRun; // TODO: Holdover until UI overhaul

    void Awake()
    {
        Instance = this;
        combatantInitiative = new List<InitiativeEntry>();
        activeCombatant = null;
    }

    public void Start()
    {
        EventHandler.Instance.deathEvent += RemoveCombatant;
        PartyController.Instance.updatePartyEvent += SetInitiativeDisplay;
    }

    public void Update()
    {
        //Debug.Log("New Frame");
    }

    public List<InitiativeEntry> enemies => combatantInitiative.Where(entry => entry.type == CombatantType.Enemy).ToList();
    public List<InitiativeEntry> Allies => combatantInitiative.Where(entry => entry.type == CombatantType.Party || entry.type == CombatantType.Ally).ToList();

    public bool IsPartyTurn => combatActive && combatantInitiative[initiativeTurn].type == CombatantType.Party;

    public int GetCurrentAP(NPC npc)
    {
        if (!combatActive || activeCombatant != npc) return 0;
        return ActionPoints;
    }

    public void insertIntoInitiative(NPC character, CombatantType type)
    {
        if (combatantInitiative.Any(entry => entry.npc == character)) return;
        var initiative = Dice.SkillCheck(6, character.charStats.finesse);

        int idx = 0;
        while (idx < combatantInitiative.Count)
        {
            if (initiative > combatantInitiative[idx].initiative) break;
            if (initiative == combatantInitiative[idx].initiative && character.charStats.finesse > combatantInitiative[idx].npc.charStats.finesse) break;
            idx++;
        }
        combatantInitiative.Insert(idx, new InitiativeEntry() { npc=character, initiative=initiative, type=type });
        SetInitiativeDisplay();
    }

    private void SetInitiativeDisplay()
    {
        var initiativeNPCs = combatantInitiative.Select(e => e.npc).ToList();
        var currentList = initiativeNPCs.Skip(initiativeTurn).Concat(initiativeNPCs.Take(initiativeTurn)).ToList();
        DefaultUI.Instance.ListInitiatives(currentList);
    }

    public void InitiateCombat(List<NPC> initialCombatants = null)
    {
        if (combatActive) return;
        combatActive = true ;
        combatantInitiative = new List<InitiativeEntry>();
        foreach (var partyMember in PartyController.Instance.party)
        {
            partyMember.SetToCombat();
        }
        foreach (var combatant in initialCombatants ?? Enumerable.Empty<NPC>())
        {
            combatant.SetToCombat();
        }

        callToArms.Invoke();

        //var fightingEnemies = SceneLoader.Instance.GetCurrentSceneManagers().Values.SelectMany(m => m.npcs).Where(npc => npc.GetComponent<Enemy>() != null).Select(npc => npc.GetComponent<Enemy>()).ToList();
        //foreach (var enemy in fightingEnemies)  //TODO: possibly make this (and end combat) an event where every entity handles their own state (and reports to the combat manager)
        //{
        //    enemy.SetToCombat(); // TODO: check what enemies are out of "range"
        //    insertIntoInitiative(enemy, CombatantType.Enemy);
        //}
        //foreach (var partyMember in PartyController.Instance.party)
        //{
        //    partyMember.SetToCombat();
        //    insertIntoInitiative(partyMember, CombatantType.Party);
        //}

        initiativeTurn = 0;
        SetCombatant(initiativeTurn);
    }

    public void EndCombat(bool gameOver = false)  
    {
        Debug.Log("End Combat");
        foreach (var combatant in combatantInitiative)
        {
            combatant.npc.EndCombat();
        }
        if (!gameOver) UIController.Instance.ActivateDefaultScreen(); // TODO: Make trigger an event that will handle the update to UI
        combatActive = false;
        SelectionController.Instance.playerUnderControl = true;
    }

    public void NextTurn()
    {
        Debug.Log("Next Turn");
        if (!combatantInitiative.Any(entry => entry.npc == PartyController.Instance.playerChar)) { EndCombat(true); return; }  // TODO: handle ui and override states better
        if (!combatantInitiative.Any(entry => entry.type == CombatantType.Enemy)) { EndCombat(); return; }
        int currInitTurn = combatantInitiative.IndexOf(combatantInitiative.Where(entry => entry.npc == activeCombatant).FirstOrDefault());
        initiativeTurn = (currInitTurn + 1) % combatantInitiative.Count;
        if (initiativeTurn == 0) NewRound();

        activeCombatant.SetIdle();
        SetCombatant(initiativeTurn);
    }

    private void SetCombatant(int turn)
    {
        activeCombatant = combatantInitiative[turn].npc;
        activeCombatant.SetActiveNPC();
        ActionPoints = activeCombatant.charStats.GetCurrStat(CharStats.StatVal.actionPoints);
        bool isEnemy = combatantInitiative[turn].type == CombatantType.Enemy;
        UIController.Instance.ActivateCombatUI();
        camScript.Instance.TrackObj(activeCombatant.gameObject);
        SelectionController.Instance.playerUnderControl = combatantInitiative[turn].type == CombatantType.Party;
        if (isEnemy)
        {
            Debug.Log("Enemy Turn");
            activeCombatant.GetComponent<Enemy>().TakeAction();
        }
        else if (SelectionController.Instance.playerUnderControl)
        {
            PartyController.Instance.SelectChar(activeCombatant.GetComponent<PartyMember>());
        }
        SetInitiativeDisplay();
    }

    private void NewRound()
    {
        GameData.Instance.gameTime += 10;
    }

    public bool inAction { private set; get; }

    public void UseCombatAbility(Selectable target, CombatActionType defaultAction)
    {
        if (currentAction == null)
        {
            switch (defaultAction)
            {
                case CombatActionType.Attack:  // TODO: better handling of separate "attack" vs move
                    currentAction = activeCombatant.GetActions()[1];
                    break;
                case CombatActionType.Run:
                    currentAction = defaultRun;
                    break;
            }
        }
        if (!inAction)
        {
            Debug.Log("Inaction pass");
            if (currentAction != null && currentAction.CheckValidTarget(activeCombatant, target))
            {
                var prev = SelectionController.Instance.playerUnderControl;
                SelectionController.Instance.playerUnderControl = false;
                // TODO: grey out buttons as well
                inAction = true;
                StartCoroutine(currentAction.UseAbility(activeCombatant, target));
                //SpendActionPoints(currentAction.UseAbility(activeCombatant, target));
                SelectionController.Instance.playerUnderControl = prev;
            }
            else SelectionController.Instance.Deselect();  // Here to deselect out of range selectables for now
            currentAction = null;
        }
        DefaultUI.Instance.FillActionPoints(activeCombatant); // TODO: gets called before coroutine is finished; AP isn't spent
    }

    public void FinishAction()
    {
        inAction = false;
    }

    public void RemoveCombatant(NPC npc)
    {
        if (!combatantInitiative.Any(entry => entry.npc == npc)) return;
        if (activeCombatant == npc) NextTurn();
        combatantInitiative.Remove(combatantInitiative.Where(entry => entry.npc == npc).FirstOrDefault());
    }

    public void SetCurrentAction(AbilityAction action)
    {
        currentAction = action;
    }

    public void SpendActionPoints(int cost)
    {
        //if (!combatActive) return;
        if (cost > ActionPoints) Debug.LogError("Spending more APs than there are: " + cost + " > " + ActionPoints);
        ActionPoints -= cost;
    }

    public bool CheckActionPoints(int cost)
    {
        Debug.Log("Check AP: " + cost + " " + ActionPoints);
        return !combatActive || cost <= ActionPoints;
    }

    public void UnsetAction()
    {
        currentAction = null;
        //TODO: call this if esc and update UI
    }

}

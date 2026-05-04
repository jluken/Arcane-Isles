using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using static UnityEngine.InputSystem.DefaultInputActions;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public bool combatActive { get; private set; } = false ;

    public event Action callToArms;
    public event Action setPeace;

    public event Action combatStatUpdate;

    public struct InitiativeEntry
    {
        public Character character;
        public int initiative;
        public CombatantType type;
    }

    public enum CombatantType
    {
        Party,
        Ally,
        Enemy,
        Bystander
    }

    public List<InitiativeEntry> combatantInitiative;

    public Character activeCombatant;
    public AbilityAction currentAction;
    private int initiativeTurn;

    private float turnTravel;
    public int ActionPoints { get; private set; }

    public Sprite runIcon;
    public AbilityAction defaultRun;

    private InputActionMap uiActions;

    void Awake()
    {
        Instance = this;
        combatantInitiative = new List<InitiativeEntry>();
        activeCombatant = null;

        defaultRun = new MoveToPoint("run", runIcon);
        uiActions = InputSystem.actions.FindActionMap("UI");
    }

    public void Start()
    {
        EventHandler.Instance.deathEvent += RemoveCombatant;
        uiActions.FindAction("Cancel").performed += (sender) => UnsetAction();
    }

    public List<InitiativeEntry> enemies => combatantInitiative.Where(entry => entry.type == CombatantType.Enemy).ToList();
    public List<InitiativeEntry> Allies => combatantInitiative.Where(entry => entry.type == CombatantType.Party || entry.type == CombatantType.Ally).ToList();

    public bool IsPartyTurn => combatActive && combatantInitiative[initiativeTurn].type == CombatantType.Party;

    public int GetCurrentAP(Character character)
    {
        if (!combatActive || activeCombatant != character) return 0;
        return ActionPoints;
    }

    public void insertIntoInitiative(Character character, CombatantType type)
    {
        if (combatantInitiative.Any(entry => entry.character == character)) return;
        var initiative = Dice.SkillCheck(6, character.charStats.GetCurrStat(CharStats.StatVal.finesse));

        int idx = 0;
        while (idx < combatantInitiative.Count)
        {
            if (initiative > combatantInitiative[idx].initiative) break;
            if (initiative == combatantInitiative[idx].initiative && character.charStats.GetCurrStat(CharStats.StatVal.finesse) > combatantInitiative[idx].character.charStats.GetCurrStat(CharStats.StatVal.finesse)) break;
            idx++;
        }
        combatantInitiative.Insert(idx, new InitiativeEntry() { character=character, initiative=initiative, type=type });
        combatStatUpdate.Invoke();
    }

    public List<Character> GetInitiativeOrder()
    {
        var initiativeChars = combatantInitiative.Select(e => e.character).ToList();
        var currentList = initiativeChars.Skip(initiativeTurn).Concat(initiativeChars.Take(initiativeTurn)).ToList();
        return currentList;
    }

    public void InitiateCombat(List<Character> initialCombatants = null)
    {
        if (combatActive) return;
        combatActive = true ;
        combatantInitiative = new List<InitiativeEntry>();
        foreach (var partyMember in PartyController.Instance.party)
        {
            partyMember.SetToCombat();
        }
        foreach (var combatant in initialCombatants ?? Enumerable.Empty<Character>())
        {
            combatant.SetToCombat();
        }

        callToArms.Invoke();

        initiativeTurn = 0;
        SetCombatant(initiativeTurn);
    }

    public bool CheckForCombatEnd()
    {
        if (!combatantInitiative.Any(entry => entry.character == PartyController.Instance.playerChar) || 
            !combatantInitiative.Any(entry => entry.type == CombatantType.Enemy)) { 
            EndCombat(); 
            return true; 
        }
        return false;
    }

    public void EndCombat()
    {
        Debug.Log("End Combat");
        setPeace.Invoke();
        combatActive = false;
        SelectionController.Instance.playerUnderControl = true;
        combatStatUpdate.Invoke();
    }

    public void NextTurn()
    {
        Debug.Log("Next Turn");
        if (CheckForCombatEnd()) return;
        int currInitTurn = combatantInitiative.IndexOf(combatantInitiative.Where(entry => entry.character == activeCombatant).FirstOrDefault());
        initiativeTurn = (currInitTurn + 1) % combatantInitiative.Count;
        if (initiativeTurn == 0) NewRound();

        activeCombatant.SetIdle();
        SetCombatant(initiativeTurn);
    }

    private void SetCombatant(int turn)
    {
        activeCombatant = combatantInitiative[turn].character;
        activeCombatant.SetActiveChar();
        turnTravel = 0;
        ActionPoints = activeCombatant.charStats.GetCurrStat(CharStats.StatVal.actionPoints);
        bool isEnemy = combatantInitiative[turn].type == CombatantType.Enemy;
        UIController.Instance.ActivateCombatUI();
        camScript.Instance.TrackObj(activeCombatant.gameObject);
        SelectionController.Instance.playerUnderControl = combatantInitiative[turn].type == CombatantType.Party;
        if (isEnemy)
        { 
            activeCombatant.GetComponent<Enemy>().TakeAction();
        }
        else if (SelectionController.Instance.playerUnderControl)
        {
            PartyController.Instance.SelectChar(activeCombatant.GetComponent<PartyMember>());
        }
        combatStatUpdate.Invoke();
    }

    private void NewRound()
    {
        GameData.Instance.gameTime += 10;
    }

    public bool inAction { private set; get; }

    public void SetCurrentAction(AbilityAction action)
    {
        currentAction = action;
    }

    public void AttackTarget(Selectable target)
    {
        if(currentAction == null) currentAction = activeCombatant.GetDefaultAttack();
        currentAction.SetActor(activeCombatant);
        currentAction.SetTarget(target);
        UseCombatAbility(currentAction);
    }

    public void TargetPoint(Vector3 target)
    {
        currentAction ??= defaultRun;
        currentAction.SetActor(activeCombatant);
        currentAction.SetTarget(target);
        UseCombatAbility(currentAction);
    }

    public void UseCombatAbility(AbilityAction action)
    {
        if(inAction && activeCombatant.mover.IsMoving()) activeCombatant.mover.StopMoving(); // Movement can be overridden 
        if (!inAction)
        {
            if (action != null && action.CheckValidAction())
            {
                var prev = SelectionController.Instance.playerUnderControl;
                SelectionController.Instance.playerUnderControl = false;
                // TODO: grey out buttons as well [UI]
                StartCoroutine(action.UseAbility());
                //SpendActionPoints(currentAction.UseAbility(activeCombatant, target));
                SelectionController.Instance.playerUnderControl = prev;
            }
            else SelectionController.Instance.Deselect();  // Here to deselect out of range selectables for now
            currentAction = null;
        }
    }

    public void LogTravel(NavMeshAgent agent, float dist)
    {
        Debug.Log("Log Travel with agent " + agent + " and active combtant " + activeCombatant.mover.agent);
        int prevPipTravel = (int)Math.Floor(turnTravel / activeCombatant.charStats.runModifier);
        turnTravel += dist;
        Debug.Log("Log Travel with turn travel " + turnTravel + " and run modifier " + activeCombatant.charStats.runModifier);
        if (agent == activeCombatant.mover.agent && (int)Math.Floor(turnTravel / activeCombatant.charStats.runModifier) > prevPipTravel) SpendActionPoints(1);
        if (ActionPoints == 0) activeCombatant.mover.StopMoving();
    }

    public void LockAction()
    {
        inAction = true;
    }

    public void FinishAction()
    {
        inAction = false;
    }

    public void RemoveCombatant(Character npc)
    {
        Debug.Log("Death event: remove npc " + npc);
        if (!combatantInitiative.Any(entry => entry.character == npc)) return;
        if (activeCombatant == npc) NextTurn();
        combatantInitiative.Remove(combatantInitiative.Where(entry => entry.character == npc).FirstOrDefault());
        ResetInitiative();
        CheckForCombatEnd();
    }

    public void ResetInitiative()
    {
        for (int i = 0; i < combatantInitiative.Count; i++)
        {
            combatantInitiative[i] = new InitiativeEntry() { character = combatantInitiative[i].character, initiative = i, type = combatantInitiative[i].type };
        }
        initiativeTurn = combatantInitiative.Where(entry => entry.character == activeCombatant).FirstOrDefault().initiative;
    }

    public void SpendActionPoints(int cost)
    {
        if (!combatActive) return;
        if (cost > ActionPoints) Debug.LogError("Spending more APs than there are: " + cost + " > " + ActionPoints);
        ActionPoints -= cost;
        DefaultUI.Instance.FillActionPoints(activeCombatant);
    }

    public bool CheckActionPoints(int cost)
    {
        return !combatActive || cost <= ActionPoints;
    }

    public void UnsetAction()
    {
        currentAction = null;
        combatStatUpdate.Invoke();
    }

}

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Pipes;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.TextCore.Text;
using static UnityEngine.InputSystem.DefaultInputActions;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public bool combatActive { get; private set; } = false;

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
    public int OnDeckActionPoints { get; private set; }

    public Sprite runIcon;
    public AbilityAction defaultRun;
    public Texture2D targetCursor;
    public Texture2D attackCursor;

    public GameObject abilityRangeMarker;
    public GameObject abilityEffectMarker;

    private InputActionMap uiActions;

    void Awake()
    {
        Instance = this;
        combatantInitiative = new List<InitiativeEntry>();
        activeCombatant = null;
        currentAction = null;

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

    public int GetCurrentOnDeckAP(Character character)
    {
        if (!combatActive || activeCombatant != character) return 0;
        return OnDeckActionPoints;
    }

    public float GetMaxPath()
    {
        return GetCurrentAP(activeCombatant) * activeCombatant.charStats.runModifier;
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
        combatantInitiative.Insert(idx, new InitiativeEntry() { character = character, initiative = initiative, type = type });
        combatStatUpdate.Invoke();
    }

    public List<Character> GetInitiativeOrder()
    {
        var initiativeChars = combatantInitiative.Select(e => e.character).ToList();
        var currentList = initiativeChars.Skip(initiativeTurn).Concat(initiativeChars.Take(initiativeTurn)).ToList();
        return currentList;
    }

    private Dictionary<Character, bool> prevCarveouts;

    public void UncarveTargets()
    {
        prevCarveouts = new Dictionary<Character, bool>();
        foreach (var target in combatantInitiative)
        {
            prevCarveouts.Add(target.character, target.character.mover.planted);
            target.character.mover.DefaultAvoidance();
        }
    }

    public void RecarveTargets()
    {
        foreach (var target in prevCarveouts)
        {
            if (target.Value == true) target.Key.mover.PlantFeet();
        }
        prevCarveouts = new Dictionary<Character, bool>();
    }

    public void InitiateCombat(List<Character> initialCombatants = null)
    {
        if (combatActive) return;
        combatActive = true;
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

    private AbilityAction executingAction;

    public bool InAction() {
        return executingAction != null;
    }

    public bool Running()
    {
        return AbilityAction.RunningAction(executingAction);
    }

    public void SetCurrentAction(AbilityAction action)
    {
        Debug.Log("Set current action to " + action.actionName);
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
        if (InAction() && Running())
        {
            // TODO: this might never get called due to deselect triggering first
            Debug.Log("Stop running for retarget");
            activeCombatant.mover.StopMoving(); // Movement can be overridden 
            Debug.Log("Finish action for retarget");
            FinishAction();
        }
        if (!InAction())
        {
            if (action != null && (action.CanUseAbility() || (AbilityAction.RunningAction(action)  && action.CheckValidAction())))
            {
                var prev = SelectionController.Instance.playerUnderControl;
                SelectionController.Instance.playerUnderControl = false;
                // TODO: grey out buttons as well [UI]
                LockAction(action);
                Debug.Log("Starting action: " + action);
                StartCoroutine(action.UseAbility());
                //SpendActionPoints(currentAction.UseAbility(activeCombatant, target));
                SelectionController.Instance.playerUnderControl = prev;
            }
            else SelectionController.Instance.Deselect();  // Here to deselect out of range selectables for now
            currentAction = null;
        }
    }

    private void Update()
    {
        //Debug.Log("Current Action is null? + " + (currentAction == null ? "true" : "false"));
        //Debug.Log("Current Action: " + (currentAction == null ? "null" : currentAction.actionName));
    }

    public void PrepAttackTarget(Selectable target) // TODO: duplicate code
    {   
        var tempAction = currentAction;
        if (currentAction == null) tempAction = activeCombatant.GetDefaultAttack();
        tempAction.SetActor(activeCombatant);
        tempAction.SetTarget(target);
        UpdateCombatDisplay(tempAction);
    }

    public void PrepTargetPoint(Vector3 target) // TODO: duplicate code
    {
        var tempAction = currentAction;
        tempAction ??= defaultRun;
        tempAction.SetActor(activeCombatant);
        tempAction.SetTarget(target);
        UpdateCombatDisplay(tempAction);
    }

    public void UpdateCombatDisplay(AbilityAction action)
    {
        //Disable display fx to start
        abilityEffectMarker.SetActive(false);
        NavLine.Instance.DisableLine();
        
        if (InAction() || action == null)
        {
            OnDeckActionPoints = 0; // Don't show AP cost while acting
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else if (!InAction())
        {
            OnDeckActionPoints = action.CheckValidAction() ? action.GetActionCost() : 0;
            action.DisplayTarget();
        }
        var rangeAction = action != null ? action : currentAction;  // TODO: put this in the action itself, or special since it doesn't require point target?
        if (!InAction() && rangeAction != null && rangeAction.range > 0)
        {
            abilityRangeMarker.SetActive(true);
            abilityRangeMarker.transform.position = activeCombatant.transform.position + new Vector3(0, 1, 0);
            abilityRangeMarker.GetComponent<DecalProjector>().size = new Vector3(rangeAction.range * 2, rangeAction.range * 2, 3f);
        }
        else abilityRangeMarker.SetActive(false);
        if(!InAction() && rangeAction != null) rangeAction.DisplayTarget();
        combatStatUpdate.Invoke();
    }

    public void LogTravel(NavMeshAgent agent, float dist)
    {
        int prevPipTravel = (int)Math.Floor(turnTravel / activeCombatant.charStats.runModifier);
        turnTravel += dist;
        if (agent == activeCombatant.mover.agent && (int)Math.Floor(turnTravel / activeCombatant.charStats.runModifier) > prevPipTravel) SpendActionPoints(1);
        if (ActionPoints == 0) activeCombatant.mover.StopMoving();
    }

    public void LockAction(AbilityAction action)
    {
        executingAction = action;
    }

    public void FinishAction()
    {
        executingAction = null;
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

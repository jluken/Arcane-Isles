using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }  // TODO: include this stuff in save data

    public bool combatActive { get; private set; } = false ;

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
    public int ActionPoints { get; private set; }

    public AbilityAction defaultRun;
    private List<AbilityAction> defaultAbilities = new List<AbilityAction>();  // TODO: Holdover until UI overhaul

    private List<AbilityAction> attacks;

    void Awake()
    {
        Instance = this;
        combatantInitiative = new List<InitiativeEntry>();
        defaultAbilities.Add(defaultRun);
        activeCombatant = null;
    }

    public void Update()
    {
        //Debug.Log("New Frame");
    }

    public List<InitiativeEntry> enemies => combatantInitiative.Where(entry => entry.type == CombatantType.Enemy).ToList();
    public List<InitiativeEntry> Allies => combatantInitiative.Where(entry => entry.type == CombatantType.Party || entry.type == CombatantType.Ally).ToList();

    private void insertIntoInitiative(NPC character, CombatantType type)
    {
        var initiative = Dice.SkillCheck(6, character.charStats.finesse);

        int idx = 0;
        while (idx < combatantInitiative.Count)
        {
            if (initiative > combatantInitiative[idx].initiative) break;
            if (initiative == combatantInitiative[idx].initiative && character.charStats.finesse > combatantInitiative[idx].npc.charStats.finesse) break;
            idx++;
        }
        combatantInitiative.Insert(idx, new InitiativeEntry() { npc=character, initiative=initiative, type=type });
    }

    public void InitiateCombat()
    {
        if (combatActive) return;
        combatActive = true ;
        combatantInitiative = new List<InitiativeEntry>();
        var fightingEnemies = SceneLoader.Instance.GetCurrentSceneManagers().Values.SelectMany(m => m.npcs).Where(npc => npc.GetComponent<Enemy>() != null).Select(npc => npc.GetComponent<Enemy>()).ToList();
        foreach (var enemy in fightingEnemies)
        {
            enemy.SetToCombat(); // TODO: check what enemies are out of "range"
            insertIntoInitiative(enemy, CombatantType.Enemy);
        }
        foreach (var partyMember in PartyController.Instance.party)
        {
            partyMember.SetToCombat();
            insertIntoInitiative(partyMember, CombatantType.Party);
        }

        SetCombatant(0);
    }

    public void EndCombat()
    {
        Debug.Log("End Combat");
        foreach(var combatant in combatantInitiative)
        {
            combatant.npc.EndCombat();
        }
        UIController.Instance.ActivateDefaultScreen();
        combatActive = false;
        SelectionController.Instance.playerUnderControl = true;
    }

    public void NextTurn()
    {
        Debug.Log("Next Turn");
        if (!combatantInitiative.Any(entry => entry.type == CombatantType.Enemy)) { EndCombat(); return; }
        int currInitTurn = combatantInitiative.IndexOf(combatantInitiative.Where(entry => entry.npc == activeCombatant).FirstOrDefault());
        int initiativeTurn = (currInitTurn + 1) % combatantInitiative.Count;
        if (initiativeTurn == 0) NewRound();

        activeCombatant.SetIdle();
        SetCombatant(initiativeTurn);
    }

    private void SetCombatant(int turn)
    {
        activeCombatant = combatantInitiative[turn].npc;
        activeCombatant.SetActiveNPC();
        ActionPoints = 6 + activeCombatant.charStats.finesse;  //TODO: figure out full calculation (stat overhaul)
        bool isEnemy = combatantInitiative[turn].type == CombatantType.Enemy;
        attacks = activeCombatant.GetWeaponAbilities(); // TODO: update UI menu whenever stuff is equipped (event)
        UIController.Instance.ActivateCombatUI(defaultAbilities.Concat(attacks).ToList(), isEnemy);
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
                case CombatActionType.Attack:
                    currentAction = attacks[0];
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
    }

    public void FinishAction()
    {
        inAction = false;
    }

    public void RemoveCombatant(NPC npc)
    {
        if (!combatantInitiative.Any(entry => entry.npc == npc)) return;
        combatantInitiative.Remove(combatantInitiative.Where(entry => entry.npc == npc).FirstOrDefault());
    }

    public void SetCurrentAction(AbilityAction action)
    {
        currentAction = action;
    }

    public void SpendActionPoints(int cost)
    {
        if (!combatActive) return;
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

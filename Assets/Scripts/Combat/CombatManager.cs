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

    public AbilityAction defaultRun; // TODO: move to NPC handling
    private List<AbilityAction> defaultAbilities = new List<AbilityAction>();  // TODO: move to NPC handling

    private List<AbilityAction> attacks;

    void Awake()
    {
        Instance = this;
        combatantInitiative = new List<InitiativeEntry>();
        defaultAbilities.Add(defaultRun);
        activeCombatant = null;
    }

    public List<InitiativeEntry> enemies => combatantInitiative.Where(entry => entry.type == CombatantType.Enemy).ToList();
    public List<InitiativeEntry> Allies => combatantInitiative.Where(entry => entry.type == CombatantType.Party || entry.type == CombatantType.Ally).ToList();

    private void insertIntoInitiative(NPC character, CombatantType type)
    {
        var initiative = character.charStats.finesse + Random.Range(1, 6); // TODO: eventually handle all dice rolls in single class

        int idx = 0;
        while (idx < combatantInitiative.Count)
        {
            if (initiative < combatantInitiative[idx].initiative) break;
            if (initiative == combatantInitiative[idx].initiative && character.charStats.finesse < combatantInitiative[idx].npc.charStats.finesse) break;
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
            var follower = partyMember.charObject.GetComponent<Follower>();
            insertIntoInitiative(follower, CombatantType.Party);
        }

        activeCombatant = combatantInitiative[0].npc;
        activeCombatant.GetComponent<NavMeshAgent>().avoidancePriority = 100;
        ActionPoints = activeCombatant.charStats.finesse + 10;  //TODO: figure out full calculation (stat overhaul)
        currentAction = null;
        attacks = activeCombatant.inventory.GetWeaponAbilities();
        bool isEnemy = combatantInitiative[0].type == CombatantType.Enemy;
        UIController.Instance.ActivateCombatUI(defaultAbilities.Concat(attacks).ToList(), isEnemy);
        camScript.Instance.CenterCamera(activeCombatant.transform.position);  // TODO: need to add tracking camera
        SelectionController.Instance.playerUnderControl = combatantInitiative[0].type == CombatantType.Party;
        if (isEnemy)
        {
            Debug.Log("Enemy Turn");            
            activeCombatant.GetComponent<Enemy>().TakeAction();
        }
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

        //TODO: duplicate code (set active combatant function?)
        activeCombatant.GetComponent<NavMeshAgent>().avoidancePriority = 50;
        activeCombatant = combatantInitiative[initiativeTurn].npc;
        activeCombatant.GetComponent<NavMeshAgent>().avoidancePriority = 100;  // TODO: store these priority settings in a global somewhere (part of NPC?)
        ActionPoints = activeCombatant.charStats.finesse + 10;  //TODO: figure out full calculation (stat overhaul)
        bool isEnemy = combatantInitiative[initiativeTurn].type == CombatantType.Enemy;
        attacks = activeCombatant.inventory.GetWeaponAbilities(); // TODO: update UI menu whenever stuff is equipped (event)
        UIController.Instance.ActivateCombatUI(defaultAbilities.Concat(attacks).ToList(), isEnemy);
        camScript.Instance.CenterCamera(activeCombatant.transform.position);  // TODO: need to add tracking camera
        SelectionController.Instance.playerUnderControl = combatantInitiative[initiativeTurn].type == CombatantType.Party;
        //Debug.Log("Can click " + SelectionController.Instance.playerUnderControl + );
        if (isEnemy)
        {
            Debug.Log("Enemy Turn");
            activeCombatant.GetComponent<Enemy>().TakeAction();
        }
    }

    private void NewRound()
    {
        GameData.Instance.gameTime += 10;
    }

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
        if (currentAction != null && currentAction.CheckValidTarget(activeCombatant, target))  // TODO: if out of range, run in-range when possible
        {
            var prev = SelectionController.Instance.playerUnderControl;
            SelectionController.Instance.playerUnderControl = false;
            // TODO: grey out buttons as well
            SpendActionPoints(currentAction.UseAbility(activeCombatant, target));
            SelectionController.Instance.playerUnderControl = prev;
        }
        else SelectionController.Instance.Deselect();  // Here to deselect out of range selectables for now
        currentAction = null;
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
        if (cost > ActionPoints) Debug.LogError("Spending more APs than there are");
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

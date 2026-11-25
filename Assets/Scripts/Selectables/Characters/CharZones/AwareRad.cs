using UnityEngine;

public class AwareRad : MonoBehaviour
{
    public Character npc;
    

    void OnTriggerStay(Collider other)  // TODO: figure out "hiding" logic and when they should be "forgotten"/when effect take place (check player hide status?) [Stealth]
    {
        if (other.gameObject.GetComponent<PartyMember>() != null && !npc.AwarePlayers.Contains(other.gameObject))
        {
            if (!CombatManager.Instance.sneaking || npc.charStats.GetCurrStat(CharStats.StatVal.insight) > other.gameObject.GetComponent<PartyMember>().charStats.GetCurrStat(CharStats.StatVal.stealth))
            {  // Stealth check
                npc.AwarePlayers.Add(other.gameObject);
                if (npc.joinIn && CombatManager.Instance.combatActive) {
                    if(npc.combatantType == CombatManager.CombatantType.Bystander) npc.combatantType = npc.SideToJoin;  // TODO: behavior should probably live in state machine (combatant type or char state)
                    npc.SetToCombat();
                }
                else if (npc.combatantType == CombatManager.CombatantType.Enemy)
                {
                    npc.FightMe();  // npcs marked as enemies always fight on sight
                }
            }   
        }
    }

    private void OnTriggerExit(Collider other) // TODO: remove from hiding if stealth?
    {
        npc.AwarePlayers.Remove(other.gameObject);
    }
}

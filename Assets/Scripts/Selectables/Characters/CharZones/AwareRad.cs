using UnityEngine;

public class AwareRad : MonoBehaviour
{
    public Enemy npc;
    

    void OnTriggerStay(Collider other)  // TODO: figure out "hiding" logic and when they should be "forgotten"/when effect take place (check player hide status?) [Stealth]
    {
        if (other.gameObject.GetComponent<PartyMember>() != null && !npc.AwarePlayers.Contains(other.gameObject))
        {
            if (!CombatManager.Instance.sneaking || npc.charStats.GetCurrStat(CharStats.StatVal.insight) > other.gameObject.GetComponent<PartyMember>().charStats.GetCurrStat(CharStats.StatVal.stealth))
            {  // Stealth check
                npc.AwarePlayers.Add(other.gameObject);
                if (npc.joinIn && CombatManager.Instance.combatActive) {
                    npc.combatantType = npc.SideToJoin;
                    npc.SetToCombat();
                }
            }   
        }
    }

    private void OnTriggerExit(Collider other) // TODO: remove from hiding if stealth?
    {
        npc.AwarePlayers.Remove(other.gameObject);
    }
}

using UnityEngine;

public class AwareRad : MonoBehaviour
{
    public Enemy enemy;
    

    void OnTriggerStay(Collider other)  // TODO: figure out "hiding" logic and when they should be "forgotten"/when effect take place (check player hide status?) [Stealth]
    {
        if (other.gameObject.GetComponent<PartyMember>() != null && !enemy.AwarePlayers.Contains(other.gameObject))
        {
            if (!CombatManager.Instance.sneaking || enemy.charStats.GetCurrStat(CharStats.StatVal.insight) > other.gameObject.GetComponent<PartyMember>().charStats.GetCurrStat(CharStats.StatVal.stealth))
            {  // Stealth check
                enemy.AwarePlayers.Add(other.gameObject);
                if (enemy.isAggroed && CombatManager.Instance.combatActive) enemy.SetToCombat();
            }   
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemy.AwarePlayers.Contains(other.gameObject))
        {
            enemy.AwarePlayers.Remove(other.gameObject);
        }
    }
}

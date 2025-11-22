using UnityEngine;

public class AwareRad : MonoBehaviour
{
    public Enemy enemy;


    void OnTriggerStay(Collider other)  // TODO: figure out "hiding" logic and when they should be "forgotten"/when effect take place (check player hide status?)
    {
        if (enemy.isAggroed && other.gameObject.GetComponent<Follower>() != null && !enemy.AwarePlayers.Contains(other.gameObject))
        {
            enemy.AwarePlayers.Add(other.gameObject);
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

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AggroRad : MonoBehaviour
{
    public Enemy enemy;
    public DecalProjector visRange;

    public void Start()
    {
        var range = GetComponent<SphereCollider>().radius;
        visRange.size = new Vector3(2 * range, 2 * range, 5);
    }
    public void Update()
    {
        visRange.enabled = CombatManager.Instance.sneaking;
    }

    void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.GetComponent<PartyMember>() != null &&
            other.gameObject.GetComponent<PartyMember>().StateMachine.CurrentPlayerState != other.gameObject.GetComponent<PartyMember>().DeadState) &&
            PartyController.Instance.party.Contains(other.gameObject.GetComponent<PartyMember>())
            ) {
            if (Utils.LineOfSight(gameObject, other.gameObject) && enemy.AwarePlayers.Contains(other.gameObject))
            {
                enemy.Alert();
            }
        }
    }

}

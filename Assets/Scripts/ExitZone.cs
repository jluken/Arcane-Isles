using Unity.VectorGraphics;
using UnityEngine;

public class ExitZone : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PartyMember>() != null && SceneLoader.Instance.GetLevel() != null)
        {
            PartyController.Instance.worldMapPos = NavMapScript.Instance.GetMapLoc(SceneLoader.Instance.GetLevelName()).MapPos();
            UIController.Instance.ActivateNavMap();
        }
    }
}

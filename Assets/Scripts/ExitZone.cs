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
            Debug.Log("Level Name: " + SceneLoader.Instance.GetLevelName());
            Debug.Log("Level ins: " + NavMapScript.Instance.GetMapLoc(SceneLoader.Instance.GetLevelName()));
            PartyController.Instance.worldMapPos = NavMapScript.Instance.GetMapLoc(SceneLoader.Instance.GetLevelName()).MapPos();
            UIController.Instance.ActivateNavMap();
        }
    }
}

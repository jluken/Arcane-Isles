using System;
using UnityEngine;
using UnityEngine.AI;

public class SceneTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string sceneName;

    private void OnTriggerEnter(Collider other) 
    {
        //Debug.Log("Test enter scene trigger ");
        if (other.gameObject.GetComponent<PartyMember>() != null && !SceneLoader.Instance.SceneLoaded(sceneName))
        {
            Debug.Log("Enter scene trigger " + sceneName + " by member " + other);
            StartCoroutine(SceneLoader.Instance.ActivateSubscene(sceneName));
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        Debug.Log("Test Exit scene trigger " + sceneName);
        if (other.gameObject.GetComponent<PartyMember>() != null && SceneLoader.Instance.SceneLoaded(sceneName))
        {
            Debug.Log("Exit scene trigger " + sceneName + " by member " + other);
            Debug.Log("Agent: " + other.name); 
            StartCoroutine(SceneLoader.Instance.SafeSceneHandler());  // Check all scenes
        }
    }
}

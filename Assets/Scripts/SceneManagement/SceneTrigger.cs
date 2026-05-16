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
            StartCoroutine(SceneLoader.Instance.ActivateSubscene(sceneName));
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.GetComponent<PartyMember>() != null && SceneLoader.Instance.SceneLoaded(sceneName))
        {
            StartCoroutine(SceneLoader.Instance.SafeSceneHandler());  // Check all scenes
        }
    }
}

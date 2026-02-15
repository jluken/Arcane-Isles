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
            Debug.Log("Enter scene trigger " + sceneName);
            StartCoroutine(SceneLoader.Instance.ActivateSubscene(sceneName));
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.GetComponent<PartyMember>() != null && !SceneLoader.Instance.SceneLoaded(sceneName))
    //    {
    //        Debug.Log("Inside scene trigger " + sceneName);
    //    }
    //}

    private void OnTriggerExit(Collider other)  // TODO: what if some players still in scene?
    {  // TODO: not triggered if movement happens during "pause"
        Debug.Log("TEst Exit scene trigger " + sceneName);
        if (other.gameObject.GetComponent<PartyMember>() != null && SceneLoader.Instance.SceneLoaded(sceneName))
        {
            Debug.Log("Exit scene trigger " + sceneName);
            Debug.Log("Agent: " + other.name); 
            //SceneLoader.Instance.SceneObjectManagers.Keys.ForEach(Console.WriteLine);
            StartCoroutine(SceneLoader.Instance.DeactivateSubscene(sceneName));
        }
    }
}

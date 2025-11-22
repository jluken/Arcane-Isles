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
        if (other.gameObject.GetComponent<Follower>() != null && !SceneLoader.Instance.ActiveLevelScenes.Contains(sceneName))
        {
            Debug.Log("Enter scene trigger " + sceneName);
            StartCoroutine(SceneLoader.Instance.ActivateSubscene(sceneName));
        }
    }

    private void OnTriggerExit(Collider other)  // TODO: what if some players still in scene?
    {
        if (other.gameObject.GetComponent<Follower>() != null && SceneLoader.Instance.ActiveLevelScenes.Contains(sceneName))
        {
            Debug.Log("Exit scene trigger " + sceneName);
            Debug.Log("Agent: " + other.name); 
            SceneLoader.Instance.ActiveLevelScenes.ForEach(Console.WriteLine);
            StartCoroutine(SceneLoader.Instance.DeactivateSubscene(sceneName));
        }
    }
}

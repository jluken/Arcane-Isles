using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

public class SceneObjectManager : MonoBehaviour
{
    public string sceneName;

    public List<EntityInventory> containers;
    public List<GameObject> npcs;
    public List<GameObject> GroundObjects;


    void Awake()
    {
        SceneLoader.Instance.AddSceneManager(this);
        SceneSaveData sceneData = SceneLoader.Instance.GetSceneData(sceneName);
        if (sceneData == null ) { return; } // Nothing loaded; keep default values

        foreach (EntityInventory inventory in containers)
        {
            var inventorySaveData = sceneData.Containers.FirstOrDefault(con => con.containerId == inventory.containerId);
            if (inventorySaveData != null) {
                inventory.LoadFromSaveData(inventorySaveData);
            }
        }

        foreach (GameObject npc in npcs)
        {
            if (sceneData.NPCs.Any(saveNpc => saveNpc.id == npc.name)){
                SceneSaveData.NPCData npcData = sceneData.NPCs.First(saveNpc => saveNpc.id == npc.name);
                npc.SetActive(npcData.active);
                npc.transform.position = new Vector3(npcData.pos[0], npcData.pos[1], npcData.pos[2]);
            }
        }

        //Delete existing ground objects from default
        foreach(var existGroundObj in GroundObjects)
        {
            Destroy(existGroundObj);
        }
        GroundObjects = new List<GameObject>();
        //Load in new ground objects
        foreach(var groundObj in sceneData.groundObjs)
        {
            Debug.Log("Instantiate " + groundObj.itemName);
            var newItem = Instantiate(Resources.Load<GameObject>("Prefabs/" + groundObj.itemName), gameObject.transform);
            newItem.transform.position = new Vector3(groundObj.pos[0], groundObj.pos[1], groundObj.pos[2]);
            newItem.GetComponent<ItemScript>().stackSize = groundObj.count;
            GroundObjects.Add(newItem);
        }
    }

    public void AddDroppedObject(string itemName)
    {
        var newItem = Instantiate(Resources.Load<GameObject>("Prefabs/" + itemName), gameObject.transform);
        GroundObjects.Add(newItem);
    }

    public void RemoveDroppedObject(GameObject obj) { 
        GroundObjects.Remove(obj);
        Destroy(obj);
    }
}

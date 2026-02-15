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
                SceneSaveData.NPCData npcSceneData = sceneData.NPCs.First(saveNpc => saveNpc.id == npc.name);

                if (!String.IsNullOrEmpty(npcSceneData.charData.id))  // Character data has been initialized from initial load
                {
                    var npcData = npc.GetComponent<NPC>();
                    npcData.charStats.LoadFromSaveData(npcSceneData.charData.charStatData);
                    npcData.inventory.LoadFromSaveData(npcSceneData.charData.inventory);
                    npcData.mover.agent.Warp(new Vector3(npcSceneData.charData.pos[0], npcSceneData.charData.pos[1], npcSceneData.charData.pos[2]));
                }
                npc.SetActive(npcSceneData.active);  // Can be turned on/off prior to loading, otherwise leave as default
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

    public void Start()
    {
        EventHandler.Instance.deathEvent += RemoveNPC;
    }

    public void AddDroppedObject(string itemName)
    {
        var newItem = Instantiate(Resources.Load<GameObject>("Prefabs/" + itemName), gameObject.transform);
        GroundObjects.Add(newItem);
    }

    public void DeleteDroppedObject(GameObject obj) { 
        GroundObjects.Remove(obj);
        Destroy(obj);
    }

    public void RemoveNPC(NPC npc)
    {
        if(npcs.Contains(npc.gameObject)) npcs.Remove(npc.gameObject);
        //Destroy(npc);
    }

    public void DisableNPC(GameObject npc)
    {
        npc.SetActive(false);
    }

    public void EnableDisableNPC(string npcName, bool enable)
    {
        var npcObj =  npcs.First(npc => npc.name == npc.name);  //TODO: log error if not present
        npcObj.SetActive(enable);
    }
}

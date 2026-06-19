using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneObjectManager : MonoBehaviour
{
    public string sceneName;

    public List<EntityInventory> containers;
    public List<GameObject> npcs;
    public List<GameObject> GroundObjects;


    protected virtual void Start()
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

        // NPCs are unique enough that they aren't loaded from prefabs, but all possible NPCs are already inside a scene and then which ones are active is determined by settings/save
        foreach (GameObject npc in npcs)
        {
            foreach (var i in sceneData.NPCs) Debug.Log(i.id);
            if (sceneData.NPCs.Any(saveNpc => saveNpc.id == npc.name))
            {
                SceneSaveData.CharData npcSceneData = sceneData.NPCs.First(saveNpc => saveNpc.id == npc.name);

                if (!String.IsNullOrEmpty(npcSceneData.charData.id))  // Character data has been initialized from initial load and possibly altered from default state
                {
                    var npcData = npc.GetComponent<Character>();
                    npcData.LoadFromSaveData(npcSceneData.charData);
                }
                npc.SetActive(npcSceneData.active);  // Can be turned on/off prior to loading, otherwise leave as default
            }
            else Destroy(npc);  // NPC totally removed from scene and moved somewhere else (eg recruited)
        }
        npcs.RemoveAll(npc => npc == null || !sceneData.NPCs.Any(saveNpc => saveNpc.id == npc.name));

        //Delete existing ground objects from default
        foreach (var existGroundObj in GroundObjects)
        {
            Destroy(existGroundObj);
        }
        GroundObjects = new List<GameObject>();
        //Load in new ground objects
        foreach(var groundObj in sceneData.groundObjs)
        {
            var newItem = Instantiate(Resources.Load<GameObject>("Prefabs/" + groundObj.itemPrefabName), gameObject.transform);
            newItem.transform.position = new Vector3(groundObj.pos[0], groundObj.pos[1], groundObj.pos[2]);
            newItem.transform.rotation = Quaternion.identity * Quaternion.Euler(groundObj.rot[0], groundObj.rot[1], groundObj.rot[2]);
            newItem.GetComponent<DroppableItem>().itemData = Resources.Load<InventoryData>("Scriptables/" + groundObj.itemDataName);
            newItem.GetComponent<DroppableItem>().stackSize = groundObj.count;
            GroundObjects.Add(newItem);
        }
    }

    public void AddDroppedObject(GameObject obj, Vector3 loc)
    {
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneByName(sceneName));
        GroundObjects.Add(obj);
    }

    public void DeleteDroppedObject(GameObject obj) { 
        GroundObjects.Remove(obj);
        Destroy(obj);
    }

    public void RemoveNPC(Character npc)
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
        var npcObj =  npcs.FirstOrDefault(npc => npc.name == npc.name);
        if (npcObj != null) npcObj.SetActive(enable);
        else Debug.LogError(npcName + " not present in " + sceneName + " data");
    }
}

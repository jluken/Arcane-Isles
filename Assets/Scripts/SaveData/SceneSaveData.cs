using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneSaveData
{
    [System.Serializable]
    public struct NPCData
    {
        public string id;
        public bool active;
        public float[] pos;
        public PartyData.CharSaveData charData;
    }

    [System.Serializable]
    public struct GroundObjData
    {
        public string itemName;
        public int count;
        public float[] pos;
    }

    public bool loaded;
    public List<NPCData> NPCs;
    public List<EntityInventorySaveData> Containers;
    public List<GroundObjData> groundObjs;

    public SceneSaveData(List<GameObject> gameNPCs, List<EntityInventory> gameContainers, List<GameObject> gameGroundObjs)
    {
        NPCs = new List<NPCData>();
        foreach (var gameNPC in gameNPCs) {
            Debug.Log("Saving NPC " + gameNPC);
            var npcData = new NPCData();
            npcData.id = gameNPC.name;
            npcData.active = gameNPC.activeSelf;
            npcData.pos = new float[] { gameNPC.transform.position.x, gameNPC.transform.position.y, gameNPC.transform.position.z };
            npcData.charData = PartyData.LoadCharData(gameNPC.name, gameNPC.transform.position, gameNPC.transform.rotation.eulerAngles, gameNPC.GetComponent<NPC>(), gameNPC.GetComponent<CharStats>(), gameNPC.GetComponent<EntityInventory>());
            NPCs.Add(npcData);
        }

        Containers = new List<EntityInventorySaveData>();
        foreach (var gameContainer in gameContainers)
        {
            Containers.Add(new EntityInventorySaveData(gameContainer));
        }

        groundObjs = new List<GroundObjData>();
        foreach (var gameGroundObj in gameGroundObjs)
        {
            var groundScript = gameGroundObj.GetComponent<ItemScript>();
            Debug.Log("ground item " + gameGroundObj);
            var groundObjData = new GroundObjData();
            Debug.Log("ground item script " + groundScript);
            Debug.Log("ground item data " + groundScript.itemData);
            groundObjData.itemName = groundScript.itemData.itemName;
            groundObjData.count = groundScript.stackSize;
            groundObjData.pos = new float[] { gameGroundObj.transform.position.x, gameGroundObj.transform.position.y, gameGroundObj.transform.position.z };
            groundObjs.Add(groundObjData);
        }
        Debug.Log("SceneSave4");
    }

    public SceneSaveData(List<string> activeNPCs, List<string> inactiveNPCs)
    {
        // Used for setting NPCs to either Active or inactive, but everything else is null so it keeps default
        // Useful for establishing save states
        NPCs = new List<NPCData>();
        foreach (var gameNPC in activeNPCs)
        {
            Debug.Log("Saving active NPC " + gameNPC);
            var npcData = new NPCData();
            npcData.id = gameNPC;
            npcData.active = true;
            NPCs.Add(npcData);
        }
        foreach (var gameNPC in inactiveNPCs)
        {
            Debug.Log("Saving inactive NPC " + gameNPC);
            var npcData = new NPCData();
            npcData.id = gameNPC;
            npcData.active = false;
            NPCs.Add(npcData);
        }
    }

}

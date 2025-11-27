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
        // TODO: fill in with more interesting data as dialogue and combat added
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
        Debug.Log("SceneSave");
        NPCs = new List<NPCData>();
        foreach (var gameNPC in gameNPCs) {
            var npcData = new NPCData();
            npcData.id = gameNPC.name;
            npcData.active = gameNPC.activeSelf;
            npcData.pos = new float[] { gameNPC.transform.position.x, gameNPC.transform.position.y, gameNPC.transform.position.z };
            npcData.charData = PartyData.LoadCharData(gameNPC.name, gameNPC.transform.position, gameNPC.transform.rotation.eulerAngles, gameNPC.GetComponent<CharStats>(), gameNPC.GetComponent<EntityInventory>());
            NPCs.Add(npcData);
        }
        Debug.Log("SceneSave2");

        Containers = new List<EntityInventorySaveData>();
        foreach (var gameContainer in gameContainers)
        {
            Containers.Add(new EntityInventorySaveData(gameContainer));
        }
        Debug.Log("SceneSave3");

        groundObjs = new List<GroundObjData>();
        foreach (var gameGroundObj in gameGroundObjs)
        {
            var groundScript = gameGroundObj.GetComponent<ItemScript>();
            var groundObjData = new GroundObjData();
            groundObjData.itemName = groundScript.itemData.itemName;
            groundObjData.count = groundScript.stackSize;
            groundObjData.pos = new float[] { gameGroundObj.transform.position.x, gameGroundObj.transform.position.y, gameGroundObj.transform.position.z };
            groundObjs.Add(groundObjData);
        }
        Debug.Log("SceneSave4");
    }

}

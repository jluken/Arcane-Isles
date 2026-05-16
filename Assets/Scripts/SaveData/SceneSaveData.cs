using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneSaveData
{
    [System.Serializable]
    public struct CharData
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
    public List<CharData> NPCs;
    public List<EntityInventorySaveData> Containers;
    public List<GroundObjData> groundObjs;

    public SceneSaveData(List<GameObject> gameChars, List<EntityInventory> gameContainers, List<GameObject> gameGroundObjs)
    {
        NPCs = new List<CharData>();
        foreach (var gameChar in gameChars) {
            Debug.Log("Saving NPC " + gameChar);
            var charData = new CharData();
            charData.id = gameChar.name;
            charData.active = gameChar.activeSelf;
            charData.pos = new float[] { gameChar.transform.position.x, gameChar.transform.position.y, gameChar.transform.position.z };
            charData.charData = PartyData.LoadCharData(gameChar.name, gameChar.transform.position, gameChar.transform.rotation.eulerAngles, gameChar.GetComponent<Character>(), gameChar.GetComponent<CharStats>(), gameChar.GetComponent<EntityInventory>());
            NPCs.Add(charData);
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
            var groundObjData = new GroundObjData();
            groundObjData.itemName = groundScript.itemData.itemName;
            groundObjData.count = groundScript.stackSize;
            groundObjData.pos = new float[] { gameGroundObj.transform.position.x, gameGroundObj.transform.position.y, gameGroundObj.transform.position.z };
            groundObjs.Add(groundObjData);
        }
    }

    public SceneSaveData(List<string> activeNPCs, List<string> inactiveNPCs)
    {
        // Used for setting NPCs to either Active or inactive, but everything else is null so it keeps default
        // Useful for establishing save states
        NPCs = new List<CharData>();
        foreach (var gameNPC in activeNPCs)
        {
            Debug.Log("Saving active NPC " + gameNPC);
            var npcData = new CharData();
            npcData.id = gameNPC;
            npcData.active = true;
            NPCs.Add(npcData);
        }
        foreach (var gameNPC in inactiveNPCs)
        {
            Debug.Log("Saving inactive NPC " + gameNPC);
            var npcData = new CharData();
            npcData.id = gameNPC;
            npcData.active = false;
            NPCs.Add(npcData);
        }
    }

}

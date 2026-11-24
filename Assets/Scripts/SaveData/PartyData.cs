
using System;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PartyData
{
    [System.Serializable]
    public struct CharStatData
    {
        public string charImageName;
        public string name;
        public int level;
        public int health;
        public int magick;
        public int vigor;
        public int finesse;
        public int psyche;
        public int intimidation;
        public int intimidationBoxes;
        public int athletics;
        public int athleticsBoxes;
        public int survival;
        public int survivalBoxes;
        public int endurance;
        public int enduranceBoxes;
        public int guile;
        public int guileBoxes;
        public int precision;
        public int precisionBoxes;
        public int sleightOfHand;
        public int sleightOfHandBoxes;
        public int stealth;
        public int stealthBoxes;
        public int persuasion;
        public int persuasionBoxes;
        public int physick;
        public int physickBoxes;
        public int perception;
        public int perceptionBoxes;
        public int arcana;
        public int arcanaBoxes;
    }

    [System.Serializable]
    public struct CharSaveData
    {
        public string id;
        public float[] pos;
        public float[] rot;
        //CharStats
        public CharStatData charStatData;
        //Inventory
        public EntityInventorySaveData inventory;
        public SigilSaveData charSigils;
        public string stateName;
        public string combatantType;

        public string[] charMaterials;
        public string charMesh;
    }

    public CharSaveData[] partyMembers;

    public int xp;

    public PartyData(PartyController partyController)
    {
        xp = partyController.xp;
        partyMembers = new CharSaveData[partyController.party.Count];
        for (int i = 0; i < partyController.party.Count; i++) {
            PartyMember partyMember = partyController.party[i];
            partyMembers[i] = LoadCharData(partyMember.gameObject.name, partyMember.transform.position, partyMember.transform.rotation.eulerAngles,
                partyMember, partyMember.charStats, partyMember.inventory, partyMember.sigils);
        }
    }

    public static CharSaveData LoadCharData(string name, Vector3 pos, Vector3 EulerRot, Character npc, CharStats charStats, EntityInventory inv, CharSigils charSigils)
    {
        Debug.Log("Loading chardata for " + name);
        CharSaveData charSaveData = new CharSaveData();
        charSaveData.id = name;
        charSaveData.pos = new float[] { pos.x, pos.y, pos.z };
        charSaveData.rot = new float[] { EulerRot.x, EulerRot.y, EulerRot.z };

        charSaveData.charStatData = LoadCharStatData(charStats);

        charSaveData.inventory = new EntityInventorySaveData(inv);
        charSaveData.charSigils = new SigilSaveData(charSigils);
        charSaveData.stateName = npc.StateName();
        charSaveData.combatantType = npc.combatantType.ToString();

        Debug.Log("Material: " + npc.renderBody.GetComponent<SkinnedMeshRenderer>().sharedMaterials[0]);
        Debug.Log("Material count: " + npc.renderBody.GetComponent<SkinnedMeshRenderer>().sharedMaterials.Count());
        Debug.Log("Material solo: " + npc.renderBody.GetComponent<SkinnedMeshRenderer>().sharedMaterial);
        charSaveData.charMaterials = npc.renderBody.GetComponent<SkinnedMeshRenderer>().sharedMaterials.Select(mat => mat.name).ToArray();
        Debug.Log("Shared mesh: " + npc.renderBody.GetComponent<SkinnedMeshRenderer>().sharedMesh);
        charSaveData.charMesh = npc.renderBody.GetComponent<SkinnedMeshRenderer>().sharedMesh.name;

        return charSaveData;
    }

    public static CharStatData LoadCharStatData(CharStats charStats)
    {
        CharStatData charStatData = new CharStatData();
        charStatData.name = charStats.charName;
        charStatData.charImageName = charStats.charImage != null ? charStats.charImage.name : "";
        Debug.Log("Char image: " + charStatData.charImageName);
        charStatData.level = charStats.GetCurrStat(CharStats.StatVal.level, false);
        charStatData.health = charStats.GetCurrStat(CharStats.StatVal.health, false);
        charStatData.magick = charStats.GetCurrStat(CharStats.StatVal.magick, false);
        charStatData.vigor = charStats.GetRawStat(CharStats.StatVal.vigor);
        charStatData.finesse = charStats.GetRawStat(CharStats.StatVal.finesse);
        charStatData.intimidation = charStats.GetRawStat(CharStats.StatVal.intimidation);
        charStatData.intimidationBoxes = charStats.GetStatBoxes(CharStats.StatVal.intimidation);
        charStatData.athletics = charStats.GetRawStat(CharStats.StatVal.athletics);
        charStatData.athleticsBoxes = charStats.GetStatBoxes(CharStats.StatVal.athletics);
        charStatData.survival = charStats.GetRawStat(CharStats.StatVal.survival);
        charStatData.survivalBoxes = charStats.GetStatBoxes(CharStats.StatVal.intimidation);
        charStatData.endurance = charStats.GetRawStat(CharStats.StatVal.endurance);
        charStatData.enduranceBoxes = charStats.GetStatBoxes(CharStats.StatVal.endurance);
        charStatData.guile = charStats.GetRawStat(CharStats.StatVal.guile);
        charStatData.guileBoxes = charStats.GetStatBoxes(CharStats.StatVal.guile);
        charStatData.precision = charStats.GetRawStat(CharStats.StatVal.precision);
        charStatData.precisionBoxes = charStats.GetStatBoxes(CharStats.StatVal.precision);
        charStatData.sleightOfHand = charStats.GetRawStat(CharStats.StatVal.sleightOfHand);
        charStatData.sleightOfHandBoxes = charStats.GetStatBoxes(CharStats.StatVal.sleightOfHand);
        charStatData.stealth = charStats.GetRawStat(CharStats.StatVal.stealth);
        charStatData.stealthBoxes = charStats.GetStatBoxes(CharStats.StatVal.stealth);
        charStatData.persuasion = charStats.GetRawStat(CharStats.StatVal.persuasion);
        charStatData.persuasionBoxes = charStats.GetStatBoxes(CharStats.StatVal.persuasion);
        charStatData.physick = charStats.GetRawStat(CharStats.StatVal.physick);
        charStatData.physickBoxes = charStats.GetStatBoxes(CharStats.StatVal.physick);
        charStatData.perception = charStats.GetRawStat(CharStats.StatVal.insight);
        charStatData.perceptionBoxes = charStats.GetStatBoxes(CharStats.StatVal.insight);
        charStatData.arcana = charStats.GetRawStat(CharStats.StatVal.arcana);
        charStatData.arcanaBoxes = charStats.GetStatBoxes(CharStats.StatVal.arcana);

        return charStatData;
    }
}

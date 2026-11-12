
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
        CharSaveData charSaveData = new CharSaveData();
        charSaveData.id = name;
        charSaveData.pos = new float[] { pos.x, pos.y, pos.z };
        charSaveData.rot = new float[] { EulerRot.x, EulerRot.y, EulerRot.z };

        charSaveData.charStatData = LoadCharStatData(charStats);

        charSaveData.inventory = new EntityInventorySaveData(inv);
        charSaveData.charSigils = new SigilSaveData(charSigils);
        charSaveData.stateName = npc.StateName();

        Debug.Log("Material: " + npc.renderBody.GetComponent<SkinnedMeshRenderer>().sharedMaterials[0]);
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
        charStatData.vigor = charStats.GetCurrStat(CharStats.StatVal.vigor, false);
        charStatData.finesse = charStats.GetCurrStat(CharStats.StatVal.finesse, false);
        charStatData.intimidation = charStats.GetCurrStat(CharStats.StatVal.intimidation, false);
        charStatData.intimidationBoxes = charStats.GetStatBoxes(CharStats.StatVal.intimidation);
        charStatData.athletics = charStats.GetCurrStat(CharStats.StatVal.athletics, false);
        charStatData.athleticsBoxes = charStats.GetStatBoxes(CharStats.StatVal.athletics);
        charStatData.survival = charStats.GetCurrStat(CharStats.StatVal.survival, false);
        charStatData.survivalBoxes = charStats.GetStatBoxes(CharStats.StatVal.intimidation);
        charStatData.endurance = charStats.GetCurrStat(CharStats.StatVal.endurance, false);
        charStatData.enduranceBoxes = charStats.GetStatBoxes(CharStats.StatVal.endurance);
        charStatData.guile = charStats.GetCurrStat(CharStats.StatVal.guile, false);
        charStatData.guileBoxes = charStats.GetStatBoxes(CharStats.StatVal.guile);
        charStatData.precision = charStats.GetCurrStat(CharStats.StatVal.precision, false);
        charStatData.precisionBoxes = charStats.GetStatBoxes(CharStats.StatVal.precision);
        charStatData.sleightOfHand = charStats.GetCurrStat(CharStats.StatVal.sleightOfHand, false);
        charStatData.sleightOfHandBoxes = charStats.GetStatBoxes(CharStats.StatVal.sleightOfHand);
        charStatData.stealth = charStats.GetCurrStat(CharStats.StatVal.stealth, false);
        charStatData.stealthBoxes = charStats.GetStatBoxes(CharStats.StatVal.stealth);
        charStatData.persuasion = charStats.GetCurrStat(CharStats.StatVal.persuasion, false);
        charStatData.persuasionBoxes = charStats.GetStatBoxes(CharStats.StatVal.persuasion);
        charStatData.physick = charStats.GetCurrStat(CharStats.StatVal.physick, false);
        charStatData.physickBoxes = charStats.GetStatBoxes(CharStats.StatVal.physick);
        charStatData.perception = charStats.GetCurrStat(CharStats.StatVal.insight, false);
        charStatData.perceptionBoxes = charStats.GetStatBoxes(CharStats.StatVal.insight);
        charStatData.arcana = charStats.GetCurrStat(CharStats.StatVal.arcana, false);
        charStatData.arcanaBoxes = charStats.GetStatBoxes(CharStats.StatVal.arcana);

        return charStatData;
    }
}

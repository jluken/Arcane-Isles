
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
        public int athletics;
        public int melee;
        public int endurance;
        public int guile;
        public int precision;
        public int sleightOfHand;
        public int stealth;
        public int persuasion;
        public int survival;
        public int perception;
        public int arcana;
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
                partyMember.charStats, partyMember.inventory);
        }
    }

    public static CharSaveData LoadCharData(string name, Vector3 pos, Vector3 EulerRot, CharStats charStats, EntityInventory inv)
    {
        CharSaveData charSaveData = new CharSaveData();
        charSaveData.id = name;
        Debug.Log("Save player pos: " + pos.x + " " + pos.y + " " + pos.z);
        charSaveData.pos = new float[] { pos.x, pos.y, pos.z };
        charSaveData.rot = new float[] { EulerRot.x, EulerRot.y, EulerRot.z };

        charSaveData.charStatData = LoadCharStatData(charStats);

        charSaveData.inventory = new EntityInventorySaveData(inv);

        return charSaveData;
    }

    public static CharStatData LoadCharStatData(CharStats charStats)
    {
        CharStatData charStatData = new CharStatData();
        charStatData.name = charStats.charName;
        charStatData.charImageName = charStats.charImage != null ? charStats.charImage.name : "";  // TODO: placeholder until better system; assume always there? Default sprite?
        charStatData.level = charStats.GetCurrStat(CharStats.StatVal.level, false);
        charStatData.health = charStats.GetCurrStat(CharStats.StatVal.health, false);
        charStatData.magick = charStats.GetCurrStat(CharStats.StatVal.magick, false);
        charStatData.vigor = charStats.GetCurrStat(CharStats.StatVal.vigor, false);
        charStatData.finesse = charStats.GetCurrStat(CharStats.StatVal.finesse, false);
        charStatData.intimidation = charStats.GetCurrStat(CharStats.StatVal.intimidation, false);
        charStatData.athletics = charStats.GetCurrStat(CharStats.StatVal.athletics, false);
        charStatData.melee = charStats.GetCurrStat(CharStats.StatVal.melee, false);
        charStatData.endurance = charStats.GetCurrStat(CharStats.StatVal.endurance, false);
        charStatData.guile = charStats.GetCurrStat(CharStats.StatVal.guile, false);
        charStatData.precision = charStats.GetCurrStat(CharStats.StatVal.precision, false);
        charStatData.sleightOfHand = charStats.GetCurrStat(CharStats.StatVal.sleightOfHand, false);
        charStatData.stealth = charStats.GetCurrStat(CharStats.StatVal.stealth, false);
        charStatData.persuasion = charStats.GetCurrStat(CharStats.StatVal.persuasion, false);
        charStatData.survival = charStats.GetCurrStat(CharStats.StatVal.survival, false);
        charStatData.perception = charStats.GetCurrStat(CharStats.StatVal.perception, false);
        charStatData.arcana = charStats.GetCurrStat(CharStats.StatVal.arcana, false);

        return charStatData;
    }
}

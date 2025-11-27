
using UnityEngine;

[System.Serializable]
public class PartyData
{
    [System.Serializable]
    public struct CharStatData
    {
        public string charImageName;
        public string name;
        public int xp;
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

    public PartyData(PartyController partyController)
    {
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
        charStatData.charImageName = charStats.charImage.name;
        charStatData.xp = charStats.xp;
        charStatData.health = charStats.health;
        charStatData.magick = charStats.magick;
        charStatData.vigor = charStats.vigor;
        charStatData.finesse = charStats.psyche;
        charStatData.intimidation = charStats.intimidation;
        charStatData.athletics = charStats.athletics;
        charStatData.melee = charStats.melee;
        charStatData.endurance = charStats.endurance;
        charStatData.guile = charStats.guile;
        charStatData.precision = charStats.precision;
        charStatData.sleightOfHand = charStats.sleightOfHand;
        charStatData.stealth = charStats.stealth;
        charStatData.persuasion = charStats.persuasion;
        charStatData.survival = charStats.survival;
        charStatData.perception = charStats.perception;
        charStatData.arcana = charStats.arcana;

        return charStatData;
    }
}

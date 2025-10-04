
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
            partyMembers[i] = LoadCharData(partyMember.gameObject.name, partyMember.charObject.transform.position, partyMember.charObject.transform.rotation.eulerAngles,
                partyMember.GetComponent<CharStats>(), partyMember.GetComponent<EntityInventory>());
            //partyMembers[i].id = partyMember.gameObject.name;
            //partyMembers[i].pos = new float[] { partyMember.charObject.transform.position.x, partyMember.charObject.transform.position.y, partyMember.charObject.transform.position.z };
            //partyMembers[i].rot = new float[] { partyMember.charObject.transform.rotation.eulerAngles.x, partyMember.charObject.transform.rotation.eulerAngles.y, partyMember.charObject.transform.rotation.eulerAngles.z };

            //var memberStats = partyMember.GetComponent<CharStats>();
            //partyMembers[i].name = memberStats.charName;
            //partyMembers[i].charImageName = memberStats.charImage.name;
            //partyMembers[i].xp = memberStats.xp;
            //partyMembers[i].health = memberStats.health;
            //partyMembers[i].magick = memberStats.magick;
            //partyMembers[i].vigor = memberStats.vigor;
            //partyMembers[i].finesse = memberStats.psyche;
            //partyMembers[i].intimidation = memberStats.intimidation;
            //partyMembers[i].athletics = memberStats.athletics;
            //partyMembers[i].melee = memberStats.melee;
            //partyMembers[i].endurance = memberStats.endurance;
            //partyMembers[i].guile = memberStats.guile;
            //partyMembers[i].precision = memberStats.precision;
            //partyMembers[i].sleightOfHand = memberStats.sleightOfHand;
            //partyMembers[i].stealth = memberStats.stealth;
            //partyMembers[i].persuasion = memberStats.persuasion;
            //partyMembers[i].survival = memberStats.survival;
            //partyMembers[i].perception = memberStats.perception;
            //partyMembers[i].arcana = memberStats.arcana;

            //var memberInv = partyMember.GetComponent<EntityInventory>();
            //partyMembers[i].inventory = new EntityInventorySaveData(memberInv);
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

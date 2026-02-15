using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class CharStats : MonoBehaviour
{
    public Sprite charImage;
    public string charName;
    public string gender = "X";

    public NPC npc => gameObject.GetComponent<NPC>();

    public int level = 1;

    // Initial values: TODO: make this private but accessible
    //Abilities
    public int vigor;
    public int finesse;
    public int psyche;

    //Skill levels
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

    public static int MaxSkillVal = 5;

    // This is a list of every core and derived stat, which can be used to add modifiers
    public enum StatVal 
    {
        level,

        health,
        maxHealth,
        magick,
        maxMagick,

        vigor,
        finesse,
        psyche,

        intimidation,
        athletics,
        melee,
        endurance,

        guile,
        precision,
        sleightOfHand, 
        stealth,

        persuasion,
        perception, 
        arcana,
        survival,

        dodge,
        armor,
        actionPoints
    };

    public Dictionary<StatVal, int> statMap = new Dictionary<StatVal, int>(); // current vals 

    private List<StatVal> Attributes = new List<StatVal>() { StatVal.vigor, StatVal.finesse, StatVal.psyche};

    private static Dictionary<StatVal, StatVal> Skills = new Dictionary<StatVal, StatVal>() { //Skills with their determining Attribute
        { StatVal.athletics, StatVal.vigor },
        { StatVal.melee, StatVal.vigor },
        { StatVal.endurance, StatVal.vigor },
        { StatVal.intimidation, StatVal.vigor },
        { StatVal.guile, StatVal.finesse },
        { StatVal.precision, StatVal.finesse },
        { StatVal.sleightOfHand, StatVal.finesse },
        { StatVal.stealth, StatVal.finesse },
        { StatVal.persuasion, StatVal.psyche },
        { StatVal.perception, StatVal.psyche },
        { StatVal.arcana, StatVal.psyche },
        { StatVal.survival, StatVal.psyche },
    };

    private Dictionary<StatVal, bool> skillGrowthOpen = new Dictionary<StatVal, bool>();

    private Dictionary<int, (StatVal, int)> modifiers = new Dictionary<int, (StatVal, int)>(); // id: (stat, amount)
    private Dictionary<int, float> modifierTimeouts = new Dictionary<int, float>(); // id: (stat, endTime)

    private Random rnd;


    public void Awake()
    {
        rnd = new Random();
        setInitStats();
    }

    public void Start()
    {
    }

    public void Update()
    {
        CheckForExpiration(GameData.Instance.gameTime);
    }

    public void LoadFromSaveData(PartyData.CharStatData statData)
    {
        charName = statData.name;
        charImage = Resources.Load<Sprite>("Sprites/" + statData.charImageName);

        statMap[StatVal.level] = statData.level;

        statMap[StatVal.vigor] = statData.vigor;
        statMap[StatVal.psyche] = statData.psyche;
        statMap[StatVal.finesse] = statData.finesse;

        statMap[StatVal.intimidation] = statData.intimidation;
        statMap[StatVal.athletics] = statData.athletics;
        statMap[StatVal.melee] = statData.melee;
        statMap[StatVal.endurance] = statData.endurance;

        statMap[StatVal.guile] = statData.guile;
        statMap[StatVal.precision] = statData.precision;
        statMap[StatVal.sleightOfHand] = statData.sleightOfHand;
        statMap[StatVal.stealth] = statData.stealth;

        statMap[StatVal.survival] = statData.survival;
        statMap[StatVal.perception] = statData.perception;
        statMap[StatVal.arcana] = statData.arcana;
        statMap[StatVal.persuasion] = statData.persuasion;

        statMap[StatVal.health] = statData.health;  
        statMap[StatVal.maxHealth] = maxHealth;
        statMap[StatVal.magick] = statData.magick; 
        statMap[StatVal.maxMagick] = maxMagick;

        statMap[StatVal.dodge] = 0; //TODO: figure out formula
        statMap[StatVal.armor] = 0; //TODO: figure out formula
        statMap[StatVal.actionPoints] = actionPoints;
    }

    public void setInitStats(bool statOverride = false)
    {
        Debug.Log("loading init stats for " + charName);

        if (!statOverride && statMap.Count > 0) return; // already loaded values

        Debug.Log("loading full stats");

        statMap[StatVal.level] = level;

        statMap[StatVal.vigor] = vigor;
        statMap[StatVal.psyche] = psyche;
        statMap[StatVal.finesse] = finesse;

        statMap[StatVal.intimidation] = intimidation;
        statMap[StatVal.athletics] = athletics;
        statMap[StatVal.melee] = melee;
        statMap[StatVal.endurance] = endurance;

        statMap[StatVal.guile] = guile;
        statMap[StatVal.precision] = precision;
        statMap[StatVal.sleightOfHand] = sleightOfHand;
        statMap[StatVal.stealth] = stealth;

        statMap[StatVal.survival] = survival;
        statMap[StatVal.perception] = perception;
        statMap[StatVal.arcana] = arcana;
        statMap[StatVal.persuasion] = persuasion;

        statMap[StatVal.health] = statMap[StatVal.maxHealth] = maxHealth;
        statMap[StatVal.magick] = statMap[StatVal.maxMagick] = maxMagick;
        Debug.Log("Set health: " + statMap[StatVal.health]);

        statMap[StatVal.dodge] = 0; //TODO: figure out formula
        statMap[StatVal.armor] = 0; //TODO: figure out formula
        statMap[StatVal.actionPoints] = actionPoints;
    }

    public int GetCurrStat(StatVal stat, bool includeMods = true)
    {
        //if(stat == StatVal.actionPoints) Debug.Log("getAP: " + statMap[StatVal.actionPoints]);
        //if (!statMap.ContainsKey(stat)) Debug.Log("Filling in blind");
        int premod = 0;
        if (Skills.ContainsKey(stat)) premod = GetCurrStat(Skills[stat]) + statMap[stat];  // Skills combine with their parent attribute
        else premod = statMap[stat];
        return premod + (includeMods ? currStatMods(stat) : 0);
    }

    public static StatVal GetSkillAbility(StatVal stat) => Skills[stat];

    public int GetRawStat(StatVal stat) => statMap[stat];  // TODO: make skill specific?

    public void ChangeSkillGrowth(StatVal stat, bool open)
    {
        if (open) skillGrowthOpen[stat] = true; // TODO: maybe create "check if skill/attribute" error catch?
        else skillGrowthOpen[stat] = false;
    }

    public bool CheckOpenSkillGrowth(StatVal stat)
    {
        if(skillGrowthOpen.ContainsKey(stat) && skillGrowthOpen[stat]) return true;
        return false;
    }

    public int addModifier(StatVal stat, int amount, float duration)
    {
        int[] existingIDs = this.modifiers.Keys.ToArray();
        var newId = rnd.Next();
        while (existingIDs.Contains(newId)) newId = rnd.Next();
        modifiers[newId] = (stat, amount);
        PartyController.Instance.UpdateParty();
        // TODO: eventually allow potions to add new actions, not just stat modifiers?
        if(duration > 0) { modifierTimeouts[newId] = GameData.Instance.gameTime + duration; }
        return newId;
    }

    private void CheckForExpiration(float time)
    {
        var expired = modifierTimeouts.Where(pair => pair.Value >= time).Select(pair => pair.Key);
        foreach (var modId in expired) { 
            modifiers.Remove(modId);
            modifierTimeouts.Remove(modId);
        }
    }

    public void removeModifier(int id)
    {
        modifiers.Remove(id);
    }

    private int maxHealth => 10 + GetCurrStat(StatVal.level) * Math.Max(1, GetCurrStat(StatVal.endurance) / 2);
    private int maxMagick => GetCurrStat(StatVal.level) + 2 * GetCurrStat(StatVal.arcana);
    private int actionPoints => 10 + (2 * GetCurrStat(StatVal.finesse));

    public void updateHealth(int amount = 0)
    {
        Debug.Log("Set health3: " + statMap[StatVal.health]);
        statMap[StatVal.health] = GetCurrStat(StatVal.health) + amount;
        statMap[StatVal.health] = Math.Min(GetCurrStat(StatVal.health), GetCurrStat(StatVal.maxHealth));
        if (GetCurrStat(StatVal.health) <= 0) {
            statMap[StatVal.health] = 0;
            npc.Die();
        }
        PartyController.Instance.UpdateParty();
    }

    public void updateMagick(int amount = 0)
    {
        statMap[StatVal.magick] = GetCurrStat(StatVal.magick) + amount;
        statMap[StatVal.magick] = Math.Min(GetCurrStat(StatVal.magick), GetCurrStat(StatVal.maxMagick));
        statMap[StatVal.magick] = Math.Max(GetCurrStat(StatVal.magick), 0);
        PartyController.Instance.UpdateParty();
    }

    public void SetStat(StatVal stat, int val)
    {
        if (stat == StatVal.health) Debug.Log("Set health4: " + statMap[StatVal.health]);
        Debug.Log("Set Stat " + stat + " to " + val);
        statMap[stat] = val;
        PartyController.Instance.UpdateParty();
    }

    public void ResetSkills() { foreach (var skill in Skills.Keys) statMap[skill] = 0; }

    public int currStatMods(StatVal stat)
    {
        var statMod = 0;
        //Debug.Log(charInventory);
        var equipStats = npc.inventory.GetEquipmentStatMods();
        if (equipStats.ContainsKey(stat)) statMod += equipStats[stat];
        if (stat == StatVal.actionPoints)
        {
            float excessWeight = Math.Max(0, npc.inventory.getTotalWeight() - (100 + 10 * GetCurrStat(StatVal.athletics))); 
            statMod -= (int)Math.Ceiling(excessWeight / 10); // TODO: also make slower?
        }
        foreach (var entry in modifiers)
        {
            if (entry.Value.Item1 == stat) statMod += entry.Value.Item2;
        }
        return statMod;
    }

    public void FullRest()
    {
        GameData.Instance.gameTime += 28800; // 8 hours
        statMap[StatVal.health] = GetCurrStat(StatVal.maxHealth);
        statMap[StatVal.magick] = GetCurrStat(StatVal.maxMagick);
        // TODO: handle spell selection
    }

}

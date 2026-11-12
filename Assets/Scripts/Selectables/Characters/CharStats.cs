using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class CharStats : MonoBehaviour
{
    public Sprite charImage;
    public string charName;
    public string gender = "X";
    public int age = 20;

    public Character character => gameObject.GetComponent<Character>();

    public int level = 0;

    //Abilities
    [SerializeField] private int vigor;
    [SerializeField] private int finesse;
    [SerializeField] private int psyche;

    //Skill levels
    [SerializeField] private int intimidation;
    [SerializeField] private int athletics;
    [SerializeField] private int survival;
    [SerializeField] private int endurance;

    [SerializeField] private int guile;
    [SerializeField] private int precision;
    [SerializeField] private int sleightOfHand;
    [SerializeField] private int stealth;

    [SerializeField] private int persuasion;
    [SerializeField] private int physick;
    [SerializeField] private int insight;
    [SerializeField] private int arcana;

    public static int MaxSkillVal = 6;

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
        survival,
        endurance,

        guile,
        precision,
        sleightOfHand, 
        stealth,

        persuasion,
        insight, 
        arcana,
        physick,

        evade,
        actionPoints,
        sigilSlots
    };

    public Dictionary<StatVal, int> statMap = new Dictionary<StatVal, int>(); // current vals 
    public Dictionary<StatVal, int> skillBoxMap = new Dictionary<StatVal, int>() // Available options in level up screen
    {
        { StatVal.athletics, 1 },
        { StatVal.survival, 1 },
        { StatVal.endurance, 1 },
        { StatVal.intimidation, 1 },
        { StatVal.guile, 1 },
        { StatVal.precision, 1 },
        { StatVal.sleightOfHand, 1 },
        { StatVal.stealth, 1 },
        { StatVal.persuasion, 1 },
        { StatVal.insight, 1 },
        { StatVal.arcana, 1 },
        { StatVal.physick, 1 },
    }; 

    public static List<StatVal> Attributes = new List<StatVal>() { StatVal.vigor, StatVal.finesse, StatVal.psyche};

    private static Dictionary<StatVal, StatVal> SkillAttrMap = new Dictionary<StatVal, StatVal>() { //Skills with their determining Attribute
        { StatVal.athletics, StatVal.vigor },
        { StatVal.survival, StatVal.vigor },
        { StatVal.endurance, StatVal.vigor },
        { StatVal.intimidation, StatVal.vigor },
        { StatVal.guile, StatVal.finesse },
        { StatVal.precision, StatVal.finesse },
        { StatVal.sleightOfHand, StatVal.finesse },
        { StatVal.stealth, StatVal.finesse },
        { StatVal.persuasion, StatVal.psyche },
        { StatVal.insight, StatVal.psyche },
        { StatVal.arcana, StatVal.psyche },
        { StatVal.physick, StatVal.psyche },
    };
    private static Dictionary<StatVal, StatVal[]> AttrSkillMap = new Dictionary<StatVal, StatVal[]>() // inverse of above
    {
        { StatVal.vigor, new StatVal[] {StatVal.athletics, StatVal.survival, StatVal.endurance, StatVal.intimidation} },
        { StatVal.finesse, new StatVal[] {StatVal.guile, StatVal.precision, StatVal.sleightOfHand, StatVal.stealth} },
        { StatVal.psyche, new StatVal[] {StatVal.persuasion, StatVal.insight, StatVal.arcana, StatVal.physick} }
    };
    public static bool IsAttribute(StatVal statVal) => Attributes.Contains(statVal);

    public static List<StatVal> Skills => SkillAttrMap.Keys.ToList();
    public static bool IsSkill(StatVal stat) => SkillAttrMap.Keys.Contains(stat);

    private Dictionary<int, (StatVal, int)> modifiers = new Dictionary<int, (StatVal, int)>(); // id: (stat, amount)
    private Dictionary<int, double> modifierTimeouts = new Dictionary<int, double>(); // id: (stat, endTime)

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
        charImage = Resources.Load<Sprite>("Sprites/portraits/" + statData.charImageName);

        statMap[StatVal.level] = statData.level;

        statMap[StatVal.vigor] = statData.vigor;
        statMap[StatVal.psyche] = statData.psyche;
        statMap[StatVal.finesse] = statData.finesse;

        statMap[StatVal.intimidation] = statData.intimidation;
        statMap[StatVal.athletics] = statData.athletics;
        statMap[StatVal.survival] = statData.survival;
        statMap[StatVal.endurance] = statData.endurance;

        statMap[StatVal.guile] = statData.guile;
        statMap[StatVal.precision] = statData.precision;
        statMap[StatVal.sleightOfHand] = statData.sleightOfHand;
        statMap[StatVal.stealth] = statData.stealth;

        statMap[StatVal.physick] = statData.physick;
        statMap[StatVal.insight] = statData.perception;
        statMap[StatVal.arcana] = statData.arcana;
        statMap[StatVal.persuasion] = statData.persuasion;

        skillBoxMap[StatVal.intimidation] = statData.intimidationBoxes;
        skillBoxMap[StatVal.athletics] = statData.athleticsBoxes;
        skillBoxMap[StatVal.survival] = statData.survivalBoxes;
        skillBoxMap[StatVal.endurance] = statData.enduranceBoxes;

        skillBoxMap [StatVal.guile] = statData.guileBoxes;
        skillBoxMap[StatVal.precision] = statData.precisionBoxes;
        skillBoxMap[StatVal.sleightOfHand] = statData.sleightOfHandBoxes;
        skillBoxMap[StatVal.stealth] = statData.stealthBoxes;

        skillBoxMap[StatVal.physick] = statData.physickBoxes;
        skillBoxMap[StatVal.insight] = statData.perceptionBoxes;
        skillBoxMap[StatVal.arcana] = statData.arcanaBoxes;
        skillBoxMap[StatVal.persuasion] = statData.persuasionBoxes;

        setDerivedStats();

        statMap[StatVal.health] = statData.health;
        statMap[StatVal.magick] = statData.magick;
    }

    public static int MaxHealthCalc(int endurance) { return 12 + (2 * endurance);  }
    private int maxHealth => MaxHealthCalc(GetCurrStat(StatVal.endurance));
    public static int MaxMagickCalc(int arcana) { return 2 * arcana; }
    private int maxMagick => MaxMagickCalc(GetCurrStat(StatVal.arcana));
    private int sigilSlots => (GetCurrStat(StatVal.arcana) + 1) / 2;
    public static int ActionPointsCalc(int finesse) { return 6 + finesse; }
    private int actionPoints => ActionPointsCalc(GetCurrStat(StatVal.finesse));
    private int evade => GetCurrStat(StatVal.finesse) + 6;  // TODO: make it easier to hit? Is this used?
    public int strengthBonus => GetCurrStat(StatVal.vigor) / 2;

    public float RunModCalc(int athletics) { return athletics / 3f; }
    public float runModifier => RunModCalc(GetCurrStat(StatVal.athletics));
    public float barterModifier => GetCurrStat(StatVal.persuasion) / 6.0f;

    public void setDerivedStats()  // TODO: is this necessary? just reference directly.
    {
        statMap[StatVal.maxHealth] = maxHealth;
        statMap[StatVal.maxMagick] = maxMagick;
        statMap[StatVal.actionPoints] = actionPoints;
        statMap[StatVal.sigilSlots] = sigilSlots;
        statMap[StatVal.evade] = evade;
    }

    public void maxBars()
    {
        statMap[StatVal.health] = maxHealth;
        statMap[StatVal.magick] = maxMagick;
    }

    public void setInitStats(bool statOverride = false)
    {

        if (!statOverride && statMap.Count > 0) return; // already loaded values;

        statMap[StatVal.level] = level;

        statMap[StatVal.vigor] = vigor;
        statMap[StatVal.psyche] = psyche;
        statMap[StatVal.finesse] = finesse;

        statMap[StatVal.intimidation] = intimidation;
        skillBoxMap[StatVal.intimidation] = vigor;
        statMap[StatVal.athletics] = athletics;
        skillBoxMap[StatVal.athletics] = vigor;
        statMap[StatVal.survival] = survival;
        skillBoxMap[StatVal.survival] = vigor;
        statMap[StatVal.endurance] = endurance;
        skillBoxMap[StatVal.endurance] = vigor;

        statMap[StatVal.guile] = guile;
        skillBoxMap[StatVal.guile] = finesse;
        statMap[StatVal.precision] = precision;
        skillBoxMap[StatVal.precision] = finesse;
        statMap[StatVal.sleightOfHand] = sleightOfHand;
        skillBoxMap[StatVal.sleightOfHand] = finesse;
        statMap[StatVal.stealth] = stealth;
        skillBoxMap[StatVal.stealth] = finesse;

        statMap[StatVal.physick] = physick;
        skillBoxMap[StatVal.physick] = psyche;
        statMap[StatVal.insight] = insight;
        skillBoxMap[StatVal.insight] = psyche;
        statMap[StatVal.arcana] = arcana;
        skillBoxMap[StatVal.arcana] = psyche;
        statMap[StatVal.persuasion] = persuasion;
        skillBoxMap[StatVal.persuasion] = psyche;

        setDerivedStats();
        maxBars();
    }

    public int GetTotalSkillPoints()
    {
        return Skills.Sum(s => GetRawStat(s));
    }

    public int GetCurrStat(StatVal stat, bool includeMods = true)
    {
        int premod = 0;
        if (SkillAttrMap.ContainsKey(stat)) premod = GetCurrStat(SkillAttrMap[stat]) + statMap[stat];  // Skills combine with their parent attribute
        else premod = statMap[stat];
        return premod + (includeMods ? currStatMods(stat) : 0);
    }

    public static StatVal GetSkillAbility(StatVal stat) => SkillAttrMap[stat];

    public int GetRawStat(StatVal stat) => statMap[stat];

    public int addModifier(StatVal stat, int amount, float duration)
    {
        int[] existingIDs = this.modifiers.Keys.ToArray();
        var newId = rnd.Next();
        while (existingIDs.Contains(newId)) newId = rnd.Next();
        modifiers[newId] = (stat, amount);
        PartyController.Instance.UpdateParty();
        // TODO: eventually allow potions to add new actions, not just stat modifiers? [status effects]
        if(duration > 0) { modifierTimeouts[newId] = GameData.Instance.gameTime + duration; }
        return newId;
    }

    private void CheckForExpiration(double time)
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

    public void updateHealth(int amount = 0)
    {
        statMap[StatVal.health] = GetCurrStat(StatVal.health) + amount;
        statMap[StatVal.health] = Math.Min(GetCurrStat(StatVal.health), GetCurrStat(StatVal.maxHealth));
        if (GetCurrStat(StatVal.health) <= 0) {
            statMap[StatVal.health] = 0;
            character.Die();
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
        int statChange = val - statMap[stat];
        statMap[stat] = val;
        if (Attributes.Contains(stat))
        {  
            foreach (var skill in AttrSkillMap[stat])
            {
                skillBoxMap[skill] += statChange;
            }
        }
        PartyController.Instance.UpdateParty();
    }

    public void SetStatBoxes(StatVal stat, int val)
    {
        skillBoxMap[stat] = val;
    }

    public int GetStatBoxes(StatVal stat)
    {
        return skillBoxMap[stat];
    }

    public void ResetSkills() { foreach (var skill in SkillAttrMap.Keys) statMap[skill] = 0; }

    public int currStatMods(StatVal stat)
    {
        var statMod = 0;
        //Debug.Log(charInventory);
        var equipStats = character.inventory.GetEquipmentStatMods();
        if (equipStats.ContainsKey(stat)) statMod += equipStats[stat];
        if (stat == StatVal.finesse)
        {
            //float excessWeight = Math.Max(0, npc.inventory.getTotalWeight() - (100 + 10 * GetCurrStat(StatVal.athletics)));
            float excessWeight = Math.Max(0, character.inventory.getEquippedWeight() - (10 * GetCurrStat(StatVal.athletics)));
            statMod -= (int)Math.Ceiling(excessWeight / 10);
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
        // TODO: handle spell selection [magic/status]
    }

}

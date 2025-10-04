using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Random = System.Random;

public class CharStats : MonoBehaviour
{
    public Sprite charImage;
    public string charName;


    public int xp;

    private int[] levelThresholds = { 0, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100, 1200};

    public int health;
    public int magick;

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

    //Values used for combat
    private int dodge;
    private int armor;

    // This is a list of every core and derived stat, which can be used to add modifiers
    public enum StatVal 
    {
        xp,
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
        armor
    };

    private Dictionary<int, (StatVal, int)> modifiers = new Dictionary<int, (StatVal, int)>(); // id: (stat, amount)

    private EntityInventory charInventory;

    private Random rnd;

    //public delegate void StatEvent();
    //public event StatEvent updateStatEvent;
    public delegate void DeathEvent();
    public event DeathEvent deathEvent;  //TODO: change party AI if dead


    public void Awake()
    {
        rnd = new Random();
        charInventory = gameObject.GetComponent<EntityInventory>();
        setBaseStats(this.vigor, this.finesse, this.psyche);
        this.health = GetCurrStat(StatVal.maxHealth);
        this.magick = GetCurrStat(StatVal.maxMagick);
    }

    public void Start()
    {
        //rnd = new Random();
        //charInventory = gameObject.GetComponent<EntityInventory>();
        //setBaseStats(this.vigor, this.finesse, this.psyche);
        //this.health = GetCurrStat(StatVal.maxHealth);
        //this.magick = GetCurrStat(StatVal.maxMagick);
    }

    public void LoadFromSaveData(PartyData.CharStatData statData)
    {
        charName = statData.name;

        charImage = Resources.Load<Sprite>("Sprites/" + statData.charImageName);
        xp = statData.xp;
        health = statData.health;
        magick = statData.magick;
        vigor = statData.vigor;
        finesse = statData.psyche;
        intimidation = statData.intimidation;
        athletics = statData.athletics;
        melee = statData.melee;
        endurance = statData.endurance;
        guile = statData.guile;
        precision = statData.precision;
        sleightOfHand = statData.sleightOfHand;
        stealth = statData.stealth;
        persuasion = statData.persuasion;
        survival = statData.survival;
        perception = statData.perception;
        arcana = statData.arcana;
    }

    public void setBaseStats(int vig, int fin, int wit)
    {
        this.vigor = vig;
        this.finesse = fin;
        this.psyche = wit;
    }

    public int GetCurrStat(StatVal stat)
    {
        int premod = 0;
        switch (stat)
        {
            case StatVal.xp: premod = this.xp; break;
            case StatVal.level: premod = Array.FindIndex(levelThresholds, threshold => xp > threshold) + 1; break;
            case StatVal.health: premod = Math.Min(this.health, GetCurrStat(StatVal.maxHealth)); break;
            case StatVal.maxHealth: premod = 10 + GetCurrStat(StatVal.level) + 2 * (GetCurrStat(StatVal.endurance)); break;  // TODO: not all creatures will follow these rules for calculation
            case StatVal.magick: premod = Math.Min(this.magick, GetCurrStat(StatVal.maxMagick)); break;
            case StatVal.maxMagick: premod = GetCurrStat(StatVal.level) + 2 * (GetCurrStat(StatVal.arcana)); break;
            case StatVal.vigor: premod = this.vigor; break;
            case StatVal.finesse: premod = this.finesse; break;
            case StatVal.psyche: premod = this.psyche; break;
            case StatVal.intimidation: premod = GetCurrStat(StatVal.vigor) + this.intimidation; break;
            case StatVal.athletics: premod = GetCurrStat(StatVal.vigor) + this.athletics; break;
            case StatVal.melee: premod = GetCurrStat(StatVal.vigor) + this.melee; break;
            case StatVal.endurance: premod = GetCurrStat(StatVal.vigor) + this.endurance; break;
            case StatVal.guile: premod = GetCurrStat(StatVal.finesse) + this.guile; break;
            case StatVal.precision: premod = GetCurrStat(StatVal.finesse) + this.precision; break;
            case StatVal.sleightOfHand: premod = GetCurrStat(StatVal.finesse) + this.sleightOfHand; break;
            case StatVal.stealth: premod = GetCurrStat(StatVal.finesse) + this.stealth; break;
            case StatVal.persuasion: premod = GetCurrStat(StatVal.psyche) + this.persuasion; break;
            case StatVal.perception: premod = GetCurrStat(StatVal.psyche) + this.perception; break;
            case StatVal.arcana: premod = GetCurrStat(StatVal.psyche) + this.arcana; break;
            case StatVal.survival: premod = GetCurrStat(StatVal.psyche) + this.survival; break;
            case StatVal.dodge: premod = GetCurrStat(StatVal.finesse) + this.survival; break;
            case StatVal.armor: premod = 0; break;
        }
        return premod + currStatMods(stat);
    }

    public int addModifier(StatVal stat, int amount, float duration)
    {
        int[] existingIDs = this.modifiers.Keys.ToArray();
        var newId = rnd.Next();
        while (existingIDs.Contains(newId)) newId = rnd.Next();
        modifiers[newId] = (stat, amount);
        PartyController.Instance.UpdateParty();
        // TODO: create event that removes modifier after duration seconds if not None
        return newId;
    }

    public void removeModifier(int id)
    {
        modifiers.Remove(id);
    }

    public void updateHealth(int amount = 0)
    {
        this.health = GetCurrStat(StatVal.health) + amount; 
        this.health = Math.Min(this.health, GetCurrStat(StatVal.maxHealth));
        PartyController.Instance.UpdateParty();
        if (this.health <= 0) { 
            this.health = 0;
            deathEvent?.Invoke();
        }
    }

    public void updateMagick(int amount = 0)
    {
        this.magick = GetCurrStat(StatVal.magick) + amount;
        this.magick = Math.Min(this.magick, GetCurrStat(StatVal.maxMagick));
        this.magick = Math.Max(this.magick, 0);
        PartyController.Instance.UpdateParty();
    }


    //public int currPsyche()
    //{
    //    return this.basePsyche + currStatMods(InventoryData.StatToChange.psyche);
    //}

    //public int currVigor()
    //{
    //    return this.baseVigor + currStatMods(InventoryData.StatToChange.vigor);
    //}

    //public int currFinesse()
    //{
    //    return this.baseFinesse + currStatMods(InventoryData.StatToChange.finesse);
    //}

    public int currStatMods(StatVal stat)
    {
        var statMod = 0;
        //Debug.Log(charInventory);
        var equipStats = charInventory.GetEquipmentStatMods();
        if (equipStats.ContainsKey(stat)) statMod += equipStats[stat];
        foreach (var entry in modifiers)
        {
            if (entry.Value.Item1 == stat) statMod += entry.Value.Item2;
        }
        return statMod;
    }

}

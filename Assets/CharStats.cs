using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Random = System.Random;
using UnityEditor;

public class CharStats : MonoBehaviour
{
    public int level = 1;
    public int baseMaxHealth;
    public int health;
    public int baseMaxStamina;
    public int stamina;

    //Abilities
    public int baseVigor;
    public int baseFinesse;
    public int baseWit;

    //Skill levels (TODO: can put an amount of points into a skill equal to that skill's corresponding ability)
    public int prying;
    public int navigation;

    private Dictionary<int, (InventoryData.StatToChange, int)> modifiers = new Dictionary<int, (InventoryData.StatToChange, int)>(); // id: (stat, amount) // TODO: find better way to handle StatToChange dict

    private EntityInventory charInventory;

    private Random rnd;

    public Sprite charImage;
    public string charName;

    public delegate void StatEvent(); // TODO: do I need multiple void delegates?
    public event StatEvent updateStatEvent;
    public delegate void DeathEvent();
    public event DeathEvent deathEvent;


    public void Start()
    {
        rnd = new Random();
        charInventory = gameObject.GetComponent<EntityInventory>();
        setBaseStats(this.baseVigor, this.baseFinesse, this.baseWit);
        //updateStatEvent = null;
    }

    public void setBaseStats(int vig, int fin, int wit)
    {
        this.baseVigor = vig;
        this.baseFinesse = fin;
        this.baseWit = wit;

        this.baseMaxHealth = 10 + ((3 + this.baseVigor) * this.level);
        this.health = this.baseMaxHealth;
        this.baseMaxStamina = 3 + this.baseFinesse;
        this.stamina = this.baseMaxStamina;
        UpdateStats();
    }

    // TODO: get effective stats based on equipment (plus maybe stuff like skills, potions - treat like controller)
    //
    // 

    public int addModifier(InventoryData.StatToChange stat, int amount)
    {
        int[] existingIDs = this.modifiers.Keys.ToArray();
        var newId = rnd.Next();
        while (existingIDs.Contains(newId)) newId = rnd.Next();  // TODO: what if there are a TON of IDs?
        modifiers[newId] = (stat, amount);
        return newId;
    }

    public void removeModifier(int id)
    {
        modifiers.Remove(id);
    }

    public void updateHealth(int amount = 0)
    {
        this.health += amount; 
        this.health = Math.Min(this.health, currMaxHealth());
        if (this.health <= 0) { 
            this.health = 0;
            deathEvent?.Invoke();
        }
        UpdateStats();
    }

    public void updateStamina(int amount = 0)
    {
        this.stamina += amount;
        this.stamina = Math.Min(this.stamina, currMaxStamina());
        UpdateStats();
    }

    public int currHealth()
    {
        return this.health;
    }

    public int currStamina()
    {
        return this.stamina;
    }

    public int getAbility(SkillCheckManager.Ability ability)
    {
        switch (ability)
        {
            case SkillCheckManager.Ability.finesse:
                return currFinesse();
            case SkillCheckManager.Ability.wit:
                return currWit();
            case SkillCheckManager.Ability.vigor:
                return currVigor();
            default:
                return 0;
        }
    }

    public int currWit()
    {
        return this.baseWit + currStatMods(InventoryData.StatToChange.wit);
    }

    public int currVigor()
    {
        return this.baseVigor + currStatMods(InventoryData.StatToChange.vigor);
    }

    public int currFinesse()
    {
        return this.baseFinesse + currStatMods(InventoryData.StatToChange.finesse);
    }

    public int getSkill(SkillCheckManager.Skill skill)
    {
        switch (skill)
        {
            case SkillCheckManager.Skill.prying:
                return prying;
            case SkillCheckManager.Skill.navigation:
                return navigation;
            default:
                return 0;
        }
    }

    public int currMaxHealth()
    {
        return this.baseMaxHealth + currStatMods(InventoryData.StatToChange.health);
    }

    public int currMaxStamina()
    {
        return this.baseMaxStamina + currStatMods(InventoryData.StatToChange.stamina);
    }

    public int currStatMods(InventoryData.StatToChange stat)  // TODO: This should possibly return list of inventory modifiers so they can be listed separately
    {
        var statMod = 0;
        statMod += charInventory.GetStatMods()[stat];
        foreach (var entry in modifiers)
        {
            if (entry.Value.Item1 == stat) statMod += entry.Value.Item2;
        }
        return statMod;
    }

    public void UpdateStats()
    {
        Debug.Log("UpdateStatEvent");
        bool isnull = updateStatEvent == null;
        Debug.Log("is null " + isnull);
        updateStatEvent?.Invoke();
    }


    // TODO: skills beyond base abilities, determined by skill check; possibly put everything in a big dictionary of skills/stats

}

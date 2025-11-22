using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillCheckItem : Selectable
{
    //TODO: should this be decoupled from being an "item"?
    [SerializeField]
    private InventoryData itemData;
    [SerializeField]
    public SkillCheck skillCheck;
    public int stackSize = 1;

    public UnityEvent successEvents;
    public UnityEvent failEvents;


    private SkillCheckManager skillCheckManager;

    public SelectionData selectSkillCheck;

    // Start is called before the first frame update
    public override void Start()
    {
        skillCheckManager = SkillCheckManager.Instance;

        selectSkillCheck = new SelectionData(this)
        {
            actionName = "Check Item",
            setSelect = true,
            interaction = new SkillCheckInteract()
        };

        base.Start();
    }

    public override List<SelectionData> Actions()
    {
        var acts = new List<SelectionData>() {selectSkillCheck };
        return acts;
    }

    public void SkillCheck()
    {
        Debug.Log("Activate check item");
        SkillCheckManager.successEvent += successListener;
        SkillCheckManager.failEvent += failListener;
        skillCheck.CheckSkill();
        UnsetInteraction();
    }

    public void successListener()
    {
        successEvents.Invoke();
    }

    public void failListener()
    {
        failEvents.Invoke();
    }
}

public class SkillCheckInteract : Interaction
{
    public override void Interact(PartyMember player, Selectable interactable)
    {
        if (interactable.GetComponent<SkillCheckItem>() == null) { Debug.LogError("Can only trade with NPCs"); }
        var skillCheckItem = interactable.GetComponent<SkillCheckItem>();
        Debug.Log("Activate check item");
        SkillCheckManager.successEvent += skillCheckItem.successListener;
        SkillCheckManager.failEvent += skillCheckItem.failListener;
        skillCheckItem.skillCheck.CheckSkill();
        skillCheckItem.UnsetInteraction();
    }
}

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
    private SkillCheck skillCheck;
    public int stackSize = 1;

    public UnityEvent successEvents;
    public UnityEvent failEvents;


    private SkillCheckManager skillCheckManager;

    // Start is called before the first frame update
    public override void Start()
    {
        skillCheckManager = GameObject.Find("SkillCheckCanvas").GetComponent<SkillCheckManager>();
        base.Start();
    }

    public override void Activate()
    {
        Debug.Log("Activate check item");
        SkillCheckManager.successEvent += successListener;
        SkillCheckManager.failEvent += failListener;
        skillCheck.CheckSkill();
        Deselect();
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

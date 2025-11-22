using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : Selectable
{

    [SerializeField]
    public InventoryData itemData;
    public int stackSize = 1;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        //Debug.Log("Droppable started");
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    public override List<SelectionData> Actions()
    {
        SelectionData pickup = new SelectionData(this)
        {
            actionName = "Pick Up",
            setSelect = true,
            interaction = new PickUp()
        };

        var acts = new List<SelectionData>();
        acts.Add(pickup);
        acts.Add(inspectSelection);
        return acts;
    }

    public class PickUp : Interaction
    {
        public override void Interact(PartyMember player, Selectable interactable)
        {
            if (interactable.GetComponent<ItemScript>() == null) { Debug.LogError("Can only Pick up droppable items"); }
            var droppable = interactable.GetComponent<ItemScript>();
            int leftOver = player.GetComponent<EntityInventory>().AddNewItem(droppable.itemData, droppable.stackSize);
            droppable.stackSize = leftOver;
            if (droppable.stackSize <= 0)
            {
                SceneLoader.Instance.SceneObjectManagers[droppable.gameObject.scene.name].RemoveDroppedObject(droppable.gameObject);
            }
        }
    }
}

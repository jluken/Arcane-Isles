using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : Selectable
{


    [SerializeField]
    private InventoryData itemData;
    public int stackSize = 1;


    private EntityInventory playerInventory;

    // Start is called before the first frame update
    public override void Start()
    {
        playerInventory = GameObject.Find("Player").GetComponent<EntityInventory>();  // TODO: use singleton after player/companion update
        base.Start();
        Debug.Log("Droppable started");
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    public override void Interact()
    {
        int leftOver = playerInventory.AddNewItem(itemData, stackSize);
        stackSize = leftOver;
        if (stackSize <= 0)
        {
            Destroy(gameObject);
        }
    }
}

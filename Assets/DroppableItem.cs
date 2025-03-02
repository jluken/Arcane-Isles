using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : Selectable
{


    [SerializeField]
    private InventoryData itemData;
    public int stackSize = 1;


    private InventoryManager inventoryManager;

    // Start is called before the first frame update
    public override void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Activate()
    {
        int leftOver = inventoryManager.AddNewItem(itemData, stackSize);
        stackSize = leftOver;
        if (stackSize <= 0)
        {
            Destroy(gameObject);
        }
    }
}

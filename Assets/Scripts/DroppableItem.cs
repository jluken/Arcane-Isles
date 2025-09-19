using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : Selectable
{


    [SerializeField]
    private InventoryData itemData;
    public int stackSize = 1;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Debug.Log("Droppable started");
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    public override void Interact()
    {
        int leftOver = PartyController.Instance.leaderObject.GetComponent<EntityInventory>().AddNewItem(itemData, stackSize);
        stackSize = leftOver;
        if (stackSize <= 0)
        {
            Destroy(gameObject);
        }
    }
}

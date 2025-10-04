using System;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentBeforeDrag;
    public Transform parentAfterDrag;
    public InventoryData inventoryData;
    private UIController uiController;
    //private InventoryMenu invMenu;
    

    public int stackSize;

    public int invSlot;

    public Image image;
    public TMP_Text counterText;

    public bool draggable;

    private void Start()
    {
        //invMenu = uiController.CurrentInventory();
        draggable = true;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!draggable) return;
        Debug.Log("Begin Drag " + gameObject.name);
        parentBeforeDrag = transform.parent;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling(); // Set at bottom of slot hierarchy to keep image over slots
        image.raycastTarget = false;  // See through image for dropping
        var startSlot = parentBeforeDrag.gameObject.GetComponent<ItemSlot>();
        stackSize = startSlot.currentStack;
        inventoryData = startSlot.ClearItem();
        //invMenu.UpdateEntity();
        //invMenu.DeselectAllSlots();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Dragging");
        if (!draggable) return;
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("end drag " + gameObject.name);
        if (!draggable) return;
        SetToNewParent();
    }

    public void SetToNewParent()
    {
        Debug.Log("Set parent of " + inventoryData.itemName);
        
        image.raycastTarget = true;
        var newSlot = parentAfterDrag.gameObject.GetComponent<ItemSlot>();
        int roomLeft = inventoryData.maxStackSize - newSlot.currentStack;

        Debug.Log("Max size: " + inventoryData.maxStackSize);
        Debug.Log("Stack at dest: " + newSlot.currentStack);
        Debug.Log("Room left: " + roomLeft);

        if (newSlot.currentStack > 0 && inventoryData != newSlot.itemData) // Swap
        {
            Debug.Log("Swap room");
            var otherDrag = newSlot.dragObject;
            var otherStack = newSlot.currentStack;
            var otherInventoryData = newSlot.ClearItem();

            transform.SetParent(parentAfterDrag);
            transform.position = transform.parent.position;
            parentAfterDrag.gameObject.GetComponent<ItemSlot>().dragObject = gameObject;
            parentAfterDrag.gameObject.GetComponent<ItemSlot>().AddItem(inventoryData, stackSize);

            otherDrag.transform.SetParent(parentBeforeDrag);
            otherDrag.transform.position = otherDrag.transform.parent.position;
            parentBeforeDrag.gameObject.GetComponent<ItemSlot>().dragObject = otherDrag;
            parentBeforeDrag.gameObject.GetComponent<ItemSlot>().AddItem(otherInventoryData, otherStack);
        }
        else if (newSlot.currentStack == 0)  // Move
        {
            transform.SetParent(parentAfterDrag);
            transform.position = transform.parent.position;
            parentAfterDrag.gameObject.GetComponent<ItemSlot>().dragObject = gameObject;
            parentAfterDrag.gameObject.GetComponent<ItemSlot>().AddItem(inventoryData, stackSize);
        }
        else if (stackSize <= roomLeft)  // Absorbed
        {
            Debug.Log("Absorb");
            parentAfterDrag.gameObject.GetComponent<ItemSlot>().AddItem(inventoryData, stackSize);
            Destroy(gameObject);
        }
        else  // Split
        {
            Debug.Log("Split");
            Debug.Log(stackSize);
            Debug.Log(roomLeft);
            int leftOver = stackSize - roomLeft;
            int transfer = Math.Min(stackSize, roomLeft);
            parentAfterDrag.gameObject.GetComponent<ItemSlot>().AddItem(inventoryData, transfer);

            transform.SetParent(parentBeforeDrag);
            transform.position = transform.parent.position;
            parentBeforeDrag.gameObject.GetComponent<ItemSlot>().dragObject = gameObject;
            parentBeforeDrag.gameObject.GetComponent<ItemSlot>().AddItem(inventoryData, leftOver);
        }
    }
}

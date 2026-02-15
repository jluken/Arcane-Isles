using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public int slotID;
    public InventoryMenu parentMenu;  // TODO: reevaluate "parentMenu" and "slotGroup" approach
    public List<ItemSlot> slotGroup;
    
    public InventoryData itemData;
    public int currentStack = 0;
    public Sprite emptySprite;

    [SerializeField]
    private TMP_Text itemText;

    public GameObject dragObject;

    public bool itemSelected;
    public GameObject selectedShader;

    private GameObject dragPrefab;

    public virtual void Awake()
    {
        dragPrefab = Resources.Load<GameObject>("Prefabs/dragObj");
    }

    public virtual void Start()
    {
        
    }

    public virtual void AddItem(InventoryData itemData, int newStackSize = 1)
    {
        Debug.Log("ADD " + itemData.itemName + " to slot " + slotID);
        Debug.Log("drag object: " + dragObject);
        if (this.currentStack > 0 && itemData != this.itemData) throw new System.Exception("Cannot add item to a slot if different item is already there");
        if (this.currentStack >= itemData.maxStackSize && newStackSize > 0) throw new System.Exception("Item slot already at max stack size");
        if (dragObject == null)
        {
            Debug.Log("Create new draggable");
            CreateDraggable(itemData);
        }
        Debug.Log("draggable: " + dragObject);
        this.itemData = itemData;
        this.currentStack += newStackSize;
        dragObject.GetComponent<DraggableItem>().counterText.text = "";
        if (this.currentStack > 1)
        {
            dragObject.GetComponent<DraggableItem>().counterText.text = this.currentStack.ToString();
        }

        itemText.text = this.itemData.itemName; // Could just be used for counter over icon
        itemText.enabled = true;
    }

    public void CreateDraggable(InventoryData itemData) 
    {
        dragObject = Instantiate(dragPrefab, transform.position, Quaternion.identity);
        dragObject.transform.SetParent(transform);
        var draggable = dragObject.GetComponent<DraggableItem>();
        draggable.invSlot = slotID;
        draggable.image.sprite = itemData.sprite;
        draggable.counterText.text = "";
        draggable.inventoryData = itemData;
    }

    public virtual InventoryData RemoveItem(int amount = 1,  bool destroyDrag = false)
    {
        this.currentStack -= amount;
        var oldData = this.itemData;
        dragObject.GetComponent<DraggableItem>().counterText.text = this.currentStack.ToString();
        if (this.currentStack == 1)
        {
            dragObject.GetComponent<DraggableItem>().counterText.text = "";
        }
        if (this.currentStack <= 0)
        {
            ClearItem(destroyDrag);
        }
        return oldData;
    }

    public virtual InventoryData ClearItem(bool destroyDrag = false) {
        //Debug.Log("REMOVE");
        if (destroyDrag)
        {
            DestroyDraggable();
        }
        this.currentStack = 0;

        var oldData = this.itemData;
        this.itemData = null;
        //itemImage.sprite = null;
        dragObject = null;

        itemText.text = null;
        itemText.enabled = false;

        if (parentMenu != null) parentMenu.SelectItem(null, slotGroup, slotID);
        return oldData;
    }

    public void DestroyDraggable()
    {
        if (dragObject != null)
        {
            Debug.Log("FOUND DRAGGABLE IN " + slotID);
        }
        Destroy(dragObject);
        dragObject = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        if (itemSelected && this.itemData != null)
        {
            parentMenu.ActivateItem(this.itemData, slotGroup, slotID);
            parentMenu.DeselectAllSlots();
        }
        else
        {
            parentMenu.SelectItem(this.itemData, slotGroup, slotID);
            selectedShader.SetActive(true);
            itemSelected = true;
        }

        Debug.Log("Left Click");
        Debug.Log(parentMenu);
        
        
    }

    public void OnRightClick()
    {
    }

    public virtual void OnDrop(PointerEventData eventData)  // TODO: allow for toggle for list of allowable drag-to panels (after that becomes a class)
    {
        Debug.Log("DROP");
        
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        if (draggableItem.sourceSlotType != InventoryData.ItemType.misc && currentStack > 0) // Will be swapping with a restricted slot
        {
            if (draggableItem.inventoryData.itemType != draggableItem.sourceSlotType) return;
            if(draggableItem.parentBeforeDrag.gameObject.GetComponent<EquipmentSlot>() != null) // Will equip this item
            {
                if (!CombatManager.Instance.CheckActionPoints(itemData.APCost)) return;
                CombatManager.Instance.SpendActionPoints(itemData.APCost);
            }
        }
        draggableItem.parentAfterDrag = transform;
        draggableItem.invSlot = slotID;
        parentMenu.UpdateEntity();
    }
}

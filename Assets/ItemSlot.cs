using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public int slotID;
    
    public InventoryData itemData;
    public int currentStack = 0;
    public Sprite emptySprite;

    [SerializeField]
    private TMP_Text itemText;

    public GameObject dragObject;

    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;

    public bool itemSelected;
    public GameObject selectedShader;

    public GameObject dragPrefab;

    private InventoryManager inventoryManager;

    public virtual void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    public virtual void AddItem(InventoryData itemData, int newStackSize = 1, bool createDrag = false)
    {
        Debug.Log("ADD " + itemData.itemName + " to slot " + slotID);
        Debug.Log("dragg object: " + dragObject);
        if (createDrag && dragObject == null)
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
        //inventoryManager.UpdateEntity(); //TODO: fine better handling of this update (event listener?)
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
        if (this.currentStack == 0)
        {
            dragObject.GetComponent<DraggableItem>().counterText.text = "";
        }
        if (this.currentStack <= 0)
        {
            ClearItem(destroyDrag);
        }
        //inventoryManager.UpdateEntity(); //TODO: fine better handling of this update (event listener?)
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

        itemDescriptionImage.sprite = emptySprite; // TODO: why is this being handled on the slot level?
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";

        //inventoryManager.UpdateEntity(); //TODO: fine better handling of this update (event listener?)
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
        if (itemSelected && this.itemData != null && this.itemData.itemType == InventoryData.ItemType.consumable)
        {
            inventoryManager.UseItem(this.itemData);
            RemoveItem(1, true);
        }
        Debug.Log("Left Click");
        Debug.Log(inventoryManager);
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        itemSelected = true;

        if (this.itemData != null)
        {
            itemDescriptionImage.sprite = this.itemData.sprite;
            itemDescriptionNameText.text = this.itemData.itemName;
            itemDescriptionText.text = this.itemData.description;
        }
        else
        {
            itemDescriptionImage.sprite = emptySprite;
            itemDescriptionNameText.text = "";
            itemDescriptionText.text = "";
        }
    }

    public void OnRightClick()
    {
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        Debug.Log("DROP");
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        draggableItem.parentAfterDrag = transform;
        draggableItem.invSlot = slotID;
    }
}

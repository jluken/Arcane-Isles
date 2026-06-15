using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EntityInventory;
using static InventoryData;

public class EquipmentPanel : InventoryPanel
{
    private Dictionary<EquipmentInvType, EquipmentSlot> equipMapping;

    public EquipmentSlot holdMainHand;
    public EquipmentSlot holdOffHand;
    public EquipmentSlot torso;
    public EquipmentSlot coat;
    public EquipmentSlot head;
    public EquipmentSlot face;
    public EquipmentSlot legs;
    public EquipmentSlot boot;
    public EquipmentSlot neck;
    public EquipmentSlot hands;
    //public EquipmentSlot leftHand;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        equipMapping = new Dictionary<EquipmentInvType, EquipmentSlot>()
        {
            { EquipmentInvType.holdMainHand, holdMainHand },
            { EquipmentInvType.holdOffHand, holdOffHand },
            { EquipmentInvType.torso, torso },
            { EquipmentInvType.coat, coat },
            { EquipmentInvType.head, head },
            { EquipmentInvType.face, face },
            { EquipmentInvType.legs, legs },
            { EquipmentInvType.boots, boot },
            { EquipmentInvType.neck, neck },
            { EquipmentInvType.hands, hands },
           // { EquipmentInvType.leftHand, leftHand }
        };
    }

    //void Start()
    //{
    //    EquipmentSlots = new List<EquipmentSlot>();
    //}

    public override void PopulateInventory(EntityInventory entityinventory, List<InventoryPanel> dragMatches = null)
    {
        inventory = entityinventory;
        ClearInventory();
        foreach(KeyValuePair <EquipmentInvType, EquipmentSlot > kvp in equipMapping)
        {
            kvp.Value.slotID = (int)kvp.Key;
            kvp.Value.slotPanel = this;
            kvp.Value.dragMatches = dragMatches;
            var equipData = inventory.GetEquipment(kvp.Key);
            if (equipData != null) { kvp.Value.AddItem(equipData, 1); }
        }
    }

    public override void ClearInventory()
    {
        DeselectPanelSlots();
        foreach (KeyValuePair<EquipmentInvType, EquipmentSlot> kvp in equipMapping) kvp.Value.ClearItem(true, false);
    }

    public override void DeselectPanelSlots()
    {
        equipMapping.Values.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });
    }

    public override void UpdateEntity()
    {
        equipMapping.ToList().ForEach(slotKVP => inventory.SetEquipment(slotKVP.Key, slotKVP.Value.itemData));
    }
}

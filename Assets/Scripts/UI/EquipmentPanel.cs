using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static InventoryData;

public class EquipmentPanel : InventoryPanel
{
    private Dictionary<ItemType, EquipmentSlot> equipMapping;

    public EquipmentSlot HeadSlot;
    public EquipmentSlot ArmorSlot;
    public EquipmentSlot WeaponSlot;
    public EquipmentSlot BootSlot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        equipMapping = new Dictionary<ItemType, EquipmentSlot>()  //TODO: more slots
        {
            { ItemType.headwear, HeadSlot },
            { ItemType.armor, ArmorSlot },
            { ItemType.weapon, WeaponSlot },
            { ItemType.boots, BootSlot }
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
        foreach(KeyValuePair < ItemType, EquipmentSlot > kvp in equipMapping)
        {
            kvp.Value.slotPanel = this;
            kvp.Value.dragMatches = dragMatches;
            var equipData = inventory.GetEquipment(kvp.Key);
            if (equipData != null) { kvp.Value.AddItem(equipData, 1); }
        }
    }

    public override void ClearInventory()
    {
        DeselectPanelSlots();
        foreach (KeyValuePair<ItemType, EquipmentSlot> kvp in equipMapping) kvp.Value.ClearItem(true, false);
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

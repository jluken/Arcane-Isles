using NUnit.Framework;
using System.Linq;
using UnityEngine;

public class InventoryMenu : MonoBehaviour
{
    public ItemSlot[] PlayerSlots;
    public ItemSlot[] ContainerSlots;
    public ItemSlot[] EquipSlots;

    public GameObject player;  // TODO: these assignments will be handled with a controller once swapping players is functional
    private EntityInventory playerInventory;

    // TODO: low priority, handle selecting multiple at once

    private void Awake()
    {
        playerInventory = player.GetComponent<EntityInventory>();
        Debug.Log("player inv awake: " + playerInventory);
        for (int i = 0; i < PlayerSlots.Length; i++)
        {
            PlayerSlots[i].slotID = i;
        }
        for (int i = 0; i < ContainerSlots.Length; i++)
        {
            ContainerSlots[i].slotID = i;
        }
        for (int i = 0; i < EquipSlots.Length; i++)
        {
            EquipSlots[i].slotID = i;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Debug.Log("player inv start: " + playerInventory);
        //for (int i = 0; i < PlayerSlots.Length; i++)
        //{
        //    PlayerSlots[i].slotID = i;
        //}
        //for (int i = 0; i < ContainerSlots.Length; i++)
        //{
        //    ContainerSlots[i].slotID = i;
        //}
    }

    public void ClearInventorySlots()
    {
        DeselectAllSlots();
        PlayerSlots.ToList().ForEach(slot => slot.ClearItem(true));
        ContainerSlots.ToList().ForEach(slot => slot.ClearItem(true));
        EquipSlots.ToList().ForEach(slot => slot.ClearItem(true));
    }

    public void DeselectAllSlots()
    {
        PlayerSlots.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; }); 
        ContainerSlots.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });
        EquipSlots.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });
    }

    public void ActivateInventory(EntityInventory inventory, bool draggable = true)
    {
        ClearInventorySlots();

        PlayerSlots.ToList().ForEach(slot => {
            var playerSlot = playerInventory.GetInventory(slot.slotID);
            if (playerSlot.Item1 != null) { slot.AddItem(playerSlot.Item1, playerSlot.Item2, draggable); } });
        ContainerSlots.ToList().ForEach(slot => {
            var containerSlot = inventory.GetInventory(slot.slotID);
            if (containerSlot.Item1 != null) { slot.AddItem(containerSlot.Item1, containerSlot.Item2, draggable); }
        });

        EquipSlots.ToList().ForEach(slot => {
            var equipSlot = playerInventory.GetEquipment(slot.slotID);
            if (equipSlot != null) { slot.AddItem(equipSlot, 1, draggable); }
        });
    }

    public void UpdateEntity(EntityInventory inventory)
    {
        PlayerSlots.ToList().ForEach(slot => playerInventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));
        ContainerSlots.ToList().ForEach(slot => inventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));
        EquipSlots.ToList().ForEach(slot => playerInventory.SetEquipment(slot.slotID, slot.itemData));
    }
}

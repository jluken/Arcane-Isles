using NUnit.Framework;
using System.Linq;
using UnityEngine;

public abstract class InventoryMenu : MenuScreen
{
    public abstract void ClearInventorySlots();

    //public abstract void UpdateEntity(); 
    public abstract void DeselectAllSlots();

    public abstract void SelectItem(InventoryData itemData, string slotGroup, int slotID); // What happens if unselected item is clicked

    public abstract void ActivateItem(InventoryData itemData, string slotGroup, int slotID); // What happens if item is selected and you click it again
}

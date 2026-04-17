using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItem", menuName = "Scriptable Objects/WeaponItem")]
public class WeaponItem : InventoryData
{
    private void Reset()
    {
        itemType = ItemType.weapon; // Overrides the default 5f when this asset is created
    }
}

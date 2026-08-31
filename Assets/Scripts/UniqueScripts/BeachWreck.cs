using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using UnityEngine;

public class BeachWreck : MonoBehaviour
{
    public InventoryData healthPack;

    public void Start()
    {
        DialogueInterface.Instance.dialogueEvent += Salvage;
    }

    public void Salvage(string eventTag)
    {
        if (eventTag != "WreckageDive") return;

        PartyController.Instance.activePartyMember.inventory.AddNewItem(healthPack, 2);
    }
}

using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using UnityEngine;

public class BeachCorpse : MonoBehaviour
{
    public GameObject corpseSword;
    public InventoryData swordInv;

    public void Start()
    {
        DialogueInterface.Instance.dialogueEvent += GrabSword;

        if(!DialogueLua.GetVariable("Beach_Corpse_Sword").AsBool) corpseSword.SetActive(false);
    }

    public void GrabSword(string eventTag)
    {
        if (eventTag != "corpseSword") return;
            
        corpseSword.SetActive(false);
        PartyController.Instance.activePartyMember.inventory.AddNewItem(swordInv);
    }
}

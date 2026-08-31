using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using UnityEngine;

public class CorpseCrab : MonoBehaviour
{

    public void Start()
    {
        DialogueInterface.Instance.dialogueEvent += FightCrab;
        EventHandler.Instance.deathEvent += MarkDead;
    }

    public void FightCrab(string eventTag)
    {
        if (eventTag != "corpseCrab") return;

        GetComponent<Enemy>().isAggroed = true;
    }

    public void MarkDead(Character npc)
    {
        if (npc != GetComponent<Character>()) return;

        DialogueLua.SetVariable("Beach_Corpse_Sword", true);
    }
}

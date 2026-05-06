using PixelCrushers.DialogueSystem;
using UnityEngine;

public class Bug : Enemy
{
    public override void Die()
    {
        QuestLog.SetQuestEntryState("Kill the Bug", 1, "success");
        base.Die();
    }
}

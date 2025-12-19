using PixelCrushers.DialogueSystem;
using UnityEngine;

public class Bug : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    //base.Update();
    //}
    public override void Die()
    {
        QuestLog.SetQuestEntryState("Kill the Bug", 1, "success");
        base.Die();
    }
}

using PixelCrushers.DialogueSystem;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueInterface : MonoBehaviour
{

    public static DialogueInterface Instance { get; private set; }

    private NPC actor;
    private NPC conversant;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        DialogueManager.instance.conversationEnded += (sender) => { actor = null; conversant = null; };
    }

    void OnEnable()
    {
        Lua.RegisterFunction("SkillCheck", this, typeof(DialogueInterface).GetMethod("SkillCheck"));
        Lua.RegisterFunction("SetStat", this, typeof(DialogueInterface).GetMethod("SetStat"));
        Lua.RegisterFunction("Recruit", this, typeof(DialogueInterface).GetMethod("TalkRecruit"));
    }

    void OnDisable()
    {
        Lua.UnregisterFunction("SkillCheck");
        Lua.UnregisterFunction("SetStat");
        Lua.UnregisterFunction("Recruit");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartPlayerConversation(string conversationName, NPC player, NPC npc)
    {
        DialogueLua.SetActorField("Player", "Display Name", player.charStats.name);
        DialogueLua.SetActorField("NPC", "Display Name", npc.charStats.name);
        actor = player;
        conversant = npc;
        DialogueManager.StartConversation(conversationName, player.transform, npc.transform);
        //DialogueManager.Set
    }

    public void DescriptionBark(Selectable item)
    {
        DialogueLua.SetVariable("DescriptionText", item.description);
        DialogueManager.Bark("DescriptionBark", item.transform);
    }

    public static double SkillCheck(string skill)
    {
        Debug.Log("Dialogue skill check");
        if (!Enum.TryParse(skill, out CharStats.StatVal skillType)) Debug.LogError("Invalid skill type " + skill);
        Debug.Log("Dialogue skill check preview: " + PartyController.Instance.leader.charStats.GetCurrStat(skillType));
        return (double)PartyController.Instance.leader.charStats.GetCurrStat(skillType);
    }

    public static void SetStat(string skill, double value)
    {
        Debug.Log("Set Stat");
        if (!Enum.TryParse(skill, out CharStats.StatVal skillType)) Debug.LogError("Invalid skill type " + skill);
        PartyController.Instance.leader.charStats.resetStat(skillType, (int)value);
    }

    public void TalkRecruit()
    {
        Debug.Log("Recruit");
        //if (!Enum.TryParse(skill, out CharStats.StatVal skillType)) Debug.LogError("Invalid skill type " + skill);
        //PartyController.Instance.leader.charStats.resetStat(skillType, (int)value);
        var recruitAct = new Recruit();
        recruitAct.Interact(actor, conversant);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

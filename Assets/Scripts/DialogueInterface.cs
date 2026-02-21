using NUnit.Framework;
using PixelCrushers.DialogueSystem;
using System;
using System.Collections.Generic;
using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.Rendering;

public class DialogueInterface : MonoBehaviour
{

    public static DialogueInterface Instance { get; private set; }

    public AbstractBarkUI descriptionBarkUIPrefab;
    private AbstractBarkUI descriptionBarkUI;
    public AbstractBarkUI speakBarkUIPrefab;
    private AbstractBarkUI speakBarkUI;

    private List<string> chatHistory;
    public delegate void ChatEvent(List<string> currChatHistory);
    public event ChatEvent updateChatLog;

    private NPC actor;
    private NPC conversant;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        DialogueManager.instance.conversationEnded += (sender) => { actor = null; conversant = null;  };
        DialogueManager.instance.conversationLinePrepared += RecordLine;
        //DialogueManager.instance. += RecordLine;
        //DialogueSystemEvents.BarkEvents.onBarkLine += RecordLine;
        //DialogueSystemEvents.ConversationEvents.onConversationLineEnd += LogConversation;

        chatHistory = new List<string>();

        Debug.Log("ui prefab: " + speakBarkUIPrefab);
        Debug.Log("ui prefab: " + descriptionBarkUIPrefab);
        //speakBarkUI = Instantiate(speakBarkUIPrefab, transform) as AbstractBarkUI;
        //descriptionBarkUI = Instantiate(descriptionBarkUIPrefab, transform) as AbstractBarkUI;
    }

    void OnEnable()
    {
        Lua.RegisterFunction("SkillCheck", this, typeof(DialogueInterface).GetMethod("SkillCheck"));
        Lua.RegisterFunction("SetStat", this, typeof(DialogueInterface).GetMethod("SetStat"));
        Lua.RegisterFunction("Recruit", this, typeof(DialogueInterface).GetMethod("TalkRecruit"));

        Lua.RegisterFunction("StartQuest", this, typeof(DialogueInterface).GetMethod("StartQuest"));
        Lua.RegisterFunction("ActivateQuestEntry", this, typeof(DialogueInterface).GetMethod("ActivateQuestEntry"));
        Lua.RegisterFunction("FinishQuest", this, typeof(DialogueInterface).GetMethod("FinishQuest"));
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
        DialogueLua.SetActorField("Player", "Display Name", player.charStats.charName);
        DialogueManager.masterDatabase.GetActor("Player").spritePortrait = player.charStats.charImage;
        DialogueLua.SetActorField("NPC", "Display Name", npc.charStats.charName);
        DialogueManager.masterDatabase.GetActor("NPC").spritePortrait = npc.charStats.charImage;
        actor = player;
        conversant = npc;
        DialogueManager.StartConversation(conversationName, player.transform, npc.transform);
        //DialogueManager.Set
    }

    public void RecordLine(Subtitle line)
    {
        //Debug.Log("Record:");
        var speaker = PixelCrushers.DialogueSystem.CharacterInfo.GetLocalizedDisplayNameInDatabase(line.speakerInfo.nameInDatabase);
        var words = line.dialogueEntry.currentDialogueText;
        chatHistory.Add(speaker + ": " + words);
        updateChatLog.Invoke(chatHistory);
}

    public void DescriptionBark(Selectable item)
    {
        //item.GetComponent<DialogueActor>().barkUISettings.barkUI = Instantiate<AbstractBarkUI>(descriptionBarkUIPrefab, item.transform);

        //var barkUI = Instantiate<AbstractBarkUI>(descriptionBarkUIPrefab, item.transform);
        //item.GetComponent<DialogueActor>().barkUISettings.barkUI = barkUI;
        //barkUI.transform.localPosition = item.GetComponent<DialogueActor>().barkUISettings.barkUIOffset;
        //barkUI.transform.localRotation = Quaternion.identity;

        //Instantiate(item.GetComponent<DialogueActor>().barkUISettings.barkUI);
        //DialogueLua.SetVariable("DescriptionText", item.description);
        //DialogueManager.Bark("DescriptionBark", item.transform);
        chatHistory.Add(item.description);
        updateChatLog.Invoke(chatHistory);
        //item.GetComponent<DialogueActor>().barkUISettings.barkUI = speakBarkUI;
    }

    public void SpeakBark(NPC npc)
    {
        //npc.GetComponent<DialogueActor>().barkUISettings.barkUI = speakBarkUI;
        DialogueLua.SetVariable("BarkText", npc.description);
        DialogueManager.Bark("BarkBark", npc.transform);
    }

    //public void RecordBark()
    //{

    //}

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
        PartyController.Instance.leader.charStats.SetStat(skillType, (int)value);
    }

    public void TalkRecruit()
    {
        Debug.Log("Recruit");
        //if (!Enum.TryParse(skill, out CharStats.StatVal skillType)) Debug.LogError("Invalid skill type " + skill);
        //PartyController.Instance.leader.charStats.resetStat(skillType, (int)value);
        var recruitAct = new Recruit();
        recruitAct.Interact(actor, conversant);
    }


    //Wrapper classes around Quest behavior
    private string QuestEntryField(int entryNum, string field) { return "Entry " + entryNum + " " + field; }

    public void StartQuest(string questName, double initEntryNum)
    {
        QuestLog.SetQuestState(questName, QuestState.Active);
        DialogueLua.SetQuestField(questName, "StartTime", GameData.Instance.gameTime);
        if(initEntryNum > 0) ActivateQuestEntry(questName, initEntryNum);
        //TODO: pop up notification?
    }

    public void ActivateQuestEntry(string questName, double entryNumd)
    {
        int entryNum = (int)entryNumd;
        QuestLog.SetQuestEntryState(questName, entryNum, QuestState.Active);
        DialogueLua.SetQuestField(questName, QuestEntryField(entryNum, "StartTime"), GameData.Instance.gameTime);
    }

    public void FinishQuest(string questName)
    {
        QuestLog.CompleteQuest(questName);
        int totalXp = 0;
        for (int e = 1; e <= QuestLog.GetQuestEntryCount(questName); e++) {
            if (QuestLog.GetQuestEntryState(questName, e) == QuestState.Success)
            {
                totalXp += DialogueLua.GetQuestField(questName, QuestEntryField(e, "XP")).asInt;
            }
        }
        PartyController.Instance.xp += totalXp;
        //TODO: pop up notification?
    }

    public struct QuestData
    {
        public string entry;
        public double startTime;
        public QuestState state;
    }

    public List<QuestData> GetQuests()
    {
        List<QuestData> quests = new List<QuestData>();
        
        Debug.Log(QuestLog.GetAllQuests(QuestState.Success | QuestState.Failure | QuestState.Active).Length);
        Debug.Log(QuestLog.GetAllQuests(QuestState.Unassigned).Length);
        foreach (string quest in QuestLog.GetAllQuests(QuestState.Success | QuestState.Failure | QuestState.Active)){
            quests.Add(
                new QuestData() { entry=quest, 
                    startTime=DialogueLua.GetQuestField(quest, "StartTime").asFloat, 
                    state= QuestLog.GetQuestState(quest) });
        }
        return quests;
    }

    public List<QuestData> GetQuestEntries(string quest)
    {
        // Return list of (entryNum, entry JournalDesc, entry start time, status)
        List<QuestData> questEntries = new List<QuestData>();
        for (int e = 1; e <= QuestLog.GetQuestEntryCount(quest); e++)
        {
            questEntries.Add(new QuestData()
            {
                entry = DialogueLua.GetQuestField(quest, QuestEntryField(e, "JournalDesc")).AsString,
                startTime = DialogueLua.GetQuestField(quest, QuestEntryField(e, "StartTime")).asFloat,
                state = QuestLog.GetQuestEntryState(quest, e)
            });
        }
        return questEntries;
    }
}

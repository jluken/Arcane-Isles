using NUnit.Framework.Internal;
using PixelCrushers.DialogueSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogueInterface;

public class JournalScript : MenuScreen
{
    public static JournalScript Instance { get; private set; }
    public GameObject journal;

    public GameObject questList;
    public GameObject questEntryList;

    public GameObject QuestPrefab;
    public GameObject ClosedQuestPrefab;
    public GameObject QuestEntryPrefab;
    public GameObject ClosedQuestEntryPrefab;

    public Button prevQuestPage;
    public Button nextQuestPage;
    public Button prevQuestEntryPage;
    public Button nextQuestEntryPage;

    public UnityEngine.UI.Toggle hideClosedQuestToggle;

    private List<QuestData> currentQuests;
    private int pageNum;

    private bool journalOpen;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentQuests = new List<QuestData> ();
    }

    public override void DeactivateMenu()
    {
        journal.SetActive(false);
        journalOpen = false;
    }

    public override void ActivateMenu()
    {
        journal.SetActive(true);
        journalOpen = true;

        currentQuests = DialogueInterface.Instance.GetQuests().Where(q => q.startTime > 0).OrderBy(q => q.startTime).ToList();
        Debug.Log("All quest count: " + currentQuests.Count);
        pageNum = 0;
        PopulateQuests(pageNum);
    }

    public void PopulateQuests(int pageNum)
    {
        int pageFirstQuest = pageNum * 10;
        var displayQuests = hideClosedQuestToggle ? currentQuests.Where(q => q.state == QuestState.Active).ToList() : currentQuests;
        int questsOnPage = Math.Min(10, displayQuests.Count - pageFirstQuest);
        for (int q = pageFirstQuest; q < pageFirstQuest + questsOnPage; q++)
        {
            var quest = displayQuests[q];
            var qPrefab = quest.state == QuestState.Active ? QuestPrefab : ClosedQuestPrefab;
            var journalQuest = Instantiate(qPrefab, questList.transform);
            journalQuest.GetComponent<TextMeshProUGUI>().text = quest.entry + "........" + (q + 1);
            journalQuest.GetComponent<QuestEntry>().questName = quest.entry;
        }

        prevQuestPage.interactable = pageNum > 0;
        nextQuestPage.interactable = displayQuests.Count > pageFirstQuest + 10;
    }

    public void OpenQuest(string questName)
    {
        Debug.Log("Open quest: " + questName);
        var entries = DialogueInterface.Instance.GetQuestEntries(questName).Where(qe => qe.startTime > 0).OrderBy(e => e.startTime).ToList();

        foreach (var qe in entries)
        {
            Debug.Log("qe: " + qe);
            var qPrefab = qe.state == QuestState.Active ? QuestEntryPrefab : ClosedQuestEntryPrefab;
            var journalQuestEntry = Instantiate(qPrefab, questEntryList.transform);
            journalQuestEntry.GetComponent<TextMeshProUGUI>().text = qe.startTime + "\n" + qe.entry; // TODO: visuals- possibly split up date entry and journal text into separate objects
        }


        var activeEntries = entries.Where(e => e.state == QuestState.Active);
        var closedEntries = entries.Where(e => e.state == QuestState.Success || e.state == QuestState.Abandoned);
    }

    public override bool IsActive()
    {
        return journalOpen;
    }
}

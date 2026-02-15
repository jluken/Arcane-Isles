using NUnit.Framework.Internal;
using PixelCrushers.DialogueSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private List<(string, string, float, QuestState)> currentQuests;  // TODO: make this a data structure
    private int pageNum;

    private bool journalOpen;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentQuests = new List<(string, string, float, QuestState)> ();
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

        // TODO: retrieve all quests from the interface, then take the active ones and create text/button prefabs that will add 10 to a page (buttons active if valid), then do same for completed/failed
        // Font will be handled as a template which can be swapped out based on settings
        currentQuests = DialogueInterface.Instance.GetQuests().Where(q => q.Item3 > 0).OrderBy(q => q.Item3).ToList();
        Debug.Log("All quest count: " + currentQuests.Count);
        pageNum = 0;
        PopulateQuests(pageNum);
    }

    public void PopulateQuests(int pageNum)
    {
        int pageFirstQuest = pageNum * 10;
        var displayQuests = hideClosedQuestToggle ? currentQuests.Where(q => q.Item4 == QuestState.Active).ToList() : currentQuests;
        int questsOnPage = Math.Min(10, displayQuests.Count - pageFirstQuest);
        for (int q = pageFirstQuest; q < pageFirstQuest + questsOnPage; q++)
        {
            var quest = displayQuests[q];
            var qPrefab = quest.Item4 == QuestState.Active ? QuestPrefab : ClosedQuestPrefab;
            var journalQuest = Instantiate(qPrefab, questList.transform);
            journalQuest.GetComponent<TextMeshProUGUI>().text = quest.Item2 + "........" + (q + 1);
            journalQuest.GetComponent<QuestEntry>().questName = quest.Item1;
        }

        prevQuestPage.interactable = pageNum > 0;
        nextQuestPage.interactable = displayQuests.Count > pageFirstQuest + 10;
    }

    public void OpenQuest(string questName)
    {
        Debug.Log("Open quest: " + questName);
        var entries = DialogueInterface.Instance.GetQuestEntries(questName).Where(qe => qe.Item3 > 0).OrderBy(e => e.Item3).ToList();

        foreach (var qe in entries)
        {
            Debug.Log("qe: " + qe);
            var qPrefab = qe.Item4 == QuestState.Active ? QuestEntryPrefab : ClosedQuestEntryPrefab;
            var journalQuestEntry = Instantiate(qPrefab, questEntryList.transform);
            journalQuestEntry.GetComponent<TextMeshProUGUI>().text = qe.Item3 + "\n" + qe.Item2; // TODO: possibly split up date entry and journal text into separate objects
        }


        var activeEntries = entries.Where(e => e.Item4 == QuestState.Active);
        var closedEntries = entries.Where(e => e.Item4 == QuestState.Success || e.Item4 == QuestState.Abandoned);
    }

    public override bool IsActive()
    {
        return journalOpen;
    }

    public override bool overlay => true;
}

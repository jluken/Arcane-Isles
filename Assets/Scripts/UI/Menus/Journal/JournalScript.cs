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
    public GameObject questDetailList;

    public GameObject QuestPrefab;
    public GameObject ClosedQuestPrefab;
    public GameObject QuestDetailPrefab;
    public GameObject ClosedQuestDetailPrefab;

    public Button prevQuestPage;
    public Button nextQuestPage;
    public Button prevQuestDetailPage;
    public Button nextQuestDetailPage;

    public UnityEngine.UI.Toggle hideClosedQuestToggle;

    private List<QuestData> currentQuests;
    private List<GameObject> displayedQuests;
    private List<GameObject> displayedDetails;
    private int pageNum;

    private bool journalOpen;

    private static int questsPerPage = 7;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentQuests = new List<QuestData> ();
        displayedQuests = new List<GameObject>();
        displayedDetails = new List<GameObject>();
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
        int pageFirstQuest = pageNum * questsPerPage;
        var displayQuests = hideClosedQuestToggle ? currentQuests.Where(q => q.state == QuestState.Active).ToList() : currentQuests;
        int questsOnPage = Math.Min(questsPerPage, displayQuests.Count - pageFirstQuest);
        foreach (var displayedQuest in displayedQuests) Destroy(displayedQuest);
        displayedQuests.Clear();
        for (int q = pageFirstQuest; q < pageFirstQuest + questsOnPage; q++)
        {
            var quest = displayQuests[q];
            var qPrefab = QuestPrefab; // quest.state == QuestState.Active ? QuestPrefab : ClosedQuestPrefab;
            var journalQuest = Instantiate(qPrefab, questList.transform);
            //journalQuest.GetComponent<TextMeshProUGUI>().text = quest.entry + "........" + (q + 1);
            journalQuest.GetComponent<QuestName>().questName = quest.entry;
            journalQuest.GetComponent<QuestName>().nameText.text = quest.entry + "........";
            journalQuest.GetComponent<QuestName>().pageNumText.text = (q + 1).ToString();
            displayedQuests.Add(journalQuest);
        }

        prevQuestPage.interactable = pageNum > 0;
        nextQuestPage.interactable = displayQuests.Count > pageFirstQuest + questsPerPage;
        OpenQuest(null);
    }

    public void OpenQuest(string questName)
    {
        Debug.Log("Open quest: " + questName);
        var questDetails = DialogueInterface.Instance.GetQuestDetails(questName).Where(qe => qe.startTime > 0).OrderBy(e => e.startTime).ToList();
        foreach (var displayedQuest in displayedDetails) Destroy(displayedQuest);
        displayedDetails.Clear();
        foreach (var qd in questDetails)
        {
            Debug.Log("qe: " + qd);
            var qdPrefab = qd.state == QuestState.Active ? QuestDetailPrefab : ClosedQuestDetailPrefab;
            var journalQuestDetailEntry = Instantiate(qdPrefab, questDetailList.transform);
            journalQuestDetailEntry.GetComponent<QuestDetail>().dateText.text = GameData.DateString((float)qd.startTime);
            journalQuestDetailEntry.GetComponent<QuestDetail>().descriptionText.text = qd.entry;
            displayedDetails.Add(journalQuestDetailEntry);
        }


        var activeDetails = questDetails.Where(e => e.state == QuestState.Active);
        var closedDetails = questDetails.Where(e => e.state == QuestState.Success || e.state == QuestState.Abandoned);
    }

    public override bool IsActive()
    {
        return journalOpen;
    }
}

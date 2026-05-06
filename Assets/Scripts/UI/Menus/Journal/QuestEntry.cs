using UnityEngine;

public class QuestEntry : JournalEntry
{
    public string questName;

    public void SelectQuest()
    {
        JournalScript.Instance.OpenQuest(questName);
    }
}

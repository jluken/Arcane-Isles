using TMPro;
using UnityEngine;

public class QuestName : MonoBehaviour
{
    public string questName;

    public TMP_Text nameText;
    public TMP_Text pageNumText;

    public void Start()
    {
        nameText.font = UIController.Instance.cursiveFont;
        pageNumText.font = UIController.Instance.printFont;
    }

    public void SelectQuest()
    {
        JournalScript.Instance.OpenQuest(questName);
    }
}

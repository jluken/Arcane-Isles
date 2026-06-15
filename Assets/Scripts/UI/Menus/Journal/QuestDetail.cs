using TMPro;
using UnityEngine;

public class QuestDetail : MonoBehaviour
{
    public TMP_Text dateText;
    public TMP_Text descriptionText;

    public void Start()
    {
        dateText.font = UIController.Instance.cursiveFont;
        descriptionText.font = UIController.Instance.cursiveFont;
    }
}

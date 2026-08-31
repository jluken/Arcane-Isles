using UnityEngine;

public class Beach : MonoBehaviour
{
    public void Start()
    {
        EventHandler.Instance.charMovement += FirstQuest;
    }

    public void FirstQuest(Character cha, string movement)
    {
        if (cha == PartyController.Instance.activePartyMember && movement == "stand") DialogueInterface.Instance.StartQuest("Stranded", 1);
    }
}

using PixelCrushers.DialogueSystem;
using UnityEngine;

public class PirateCamp : MonoBehaviour
{

    public void Start()
    {
        DialogueInterface.Instance.dialogueEvent += RowOut;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!DialogueLua.GetVariable("SwirlingIsle.PirateCamp.Arrived").asBool && other.gameObject.GetComponent<PartyMember>() != null)
        {
            DialogueLua.SetVariable("SwirlingIsle.PirateCamp.Arrived", true);
            var player = PartyController.Instance.playerChar;
            DialogueInterface.Instance.StartPlayerObjConversation("SwirlingIsle.PirateCamp.Arrival", player, player);
        }
    }

    public void RowOut(string eventTag)
    {
        if (eventTag != "rowOut") return;

        SceneLoader.Instance.SetToLevelSpawn(null, 2);
    }
}

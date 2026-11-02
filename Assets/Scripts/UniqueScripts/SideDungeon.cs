using PixelCrushers.DialogueSystem;
using UnityEngine;

public class SideDungeon : MonoBehaviour
{

    public void Start()
    {
        //DialogueInterface.Instance.dialogueEvent += RowOut;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!DialogueLua.GetVariable("SwirlingIsle.Temple.SideSeen").asBool && other.gameObject.GetComponent<PartyMember>() != null)
        {
            DialogueLua.SetVariable("SwirlingIsle.Temple.SideSeen", true);
            var player = PartyController.Instance.playerChar;
            DialogueInterface.Instance.StartPlayerObjConversation("SwirlingIsle.Temple.EnterSide", player, player);
        }
    }

    //public void RowOut(string eventTag)
    //{
    //    if (eventTag != "rowOut") return;

    //    SceneLoader.Instance.SetToLevelSpawn(null, 2);
    //}
}

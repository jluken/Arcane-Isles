using PixelCrushers.DialogueSystem;
using Unity.VectorGraphics;
using UnityEngine;

public class SurvivorCamp : MonoBehaviour
{
    public Character doctor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (!DialogueLua.GetVariable("SwirlingIsle.Camp.Arrived").asBool && other.gameObject.GetComponent<PartyMember>() != null)
        {
            DialogueLua.SetVariable("SwirlingIsle.Camp.Arrived", true);
            DialogueInterface.SpeakBark(doctor, "Over here!");
        }
    }
}

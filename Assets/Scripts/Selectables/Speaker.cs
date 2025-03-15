using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Speaker : Selectable
{
    public GameObject textLog;
    private TextLog textLogText;
    public GameObject itemPopUp;
    private IEnumerator displayRoutine;

    public DialogueBox dialogueBox;
    public List<string> dialogue;

    private string description = "Hello World!";

    public override void Start()
    {
        textLogText = textLog.GetComponent<TextLog>();
        itemPopUp.SetActive(false);
        var height = gameObject.GetComponent<MeshRenderer>().bounds.max.y;
        itemPopUp.transform.localPosition = new Vector3(0, (height/2) + 2, 0);
        displayRoutine = DisplayText();
        base.Start();
    }

    public void Inspect()
    {

        textLogText.AddText(description);
        
        StopCoroutine(displayRoutine);
        displayRoutine = DisplayText();
        StartCoroutine(displayRoutine);
    }

    IEnumerator DisplayText()
    {
        itemPopUp.GetComponent<TMP_Text>().text = description;
        itemPopUp.SetActive(true);
        yield return new WaitForSeconds(3);
        itemPopUp.SetActive(false); // TODO: fade out animation?
    }

    public override List<(string, Action)> Actions()
    {
        var acts = new List<(string, Action)>();
        acts.Add(("Inspect", Inspect));
        acts.Add(("Talk", Talk));

        return acts;
    }

    public void Talk()
    {
        base.SetTarget();
        base.SetInteractAction(() => { dialogueBox.ActivateChat(dialogue, null); });
    }
}

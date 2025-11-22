using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Speaker : NPC  // Making a type of NPC for now
{
    //public GameObject textLog;
    private TextLog textLogText;
    //public GameObject itemPopUp;
    private IEnumerator displayRoutine;

    public override void Start()
    {
        //textLogText = textLog.GetComponent<TextLog>();
        //itemPopUp.SetActive(false);
        //var height = gameObject.GetComponent<MeshRenderer>().bounds.max.y;
        //itemPopUp.transform.localPosition = new Vector3(0, (height/2) + 2, 0);
        //displayRoutine = DisplayText();
        base.Start();
    }

    //public void Inspect()
    //{

    //    textLogText.AddText(description);
        
    //    StopCoroutine(displayRoutine);
    //    displayRoutine = DisplayText();
    //    StartCoroutine(displayRoutine);
    //}

    //IEnumerator DisplayText()
    //{
    //    itemPopUp.GetComponent<TMP_Text>().text = description;
    //    itemPopUp.SetActive(true);
    //    yield return new WaitForSeconds(3);
    //    itemPopUp.SetActive(false); // TODO: fade out animation?
    //}

    public override List<SelectionData> Actions()
    {
        var acts = new List<SelectionData>() { inspectSelection, talk} ;

        return acts;
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour // TODO: handle more complex behavior like trees and updating char pic with dialogueTree system
{
    public GameObject chatBox;
    public TMP_Text textContent;
    public GameObject speakerImg;
    public UIScript ui;

    public List<string> textEntries;

    private bool chatOpen;

    private List<string> dialogue;
    private int currDialogue;  //TODO: this is very much a placeholder and will be replaced with tree asset

    //private void Awake()
    //{
    //    ActivateChat(new List<string>() { "Prime the textbox", "iterate" }, null);
    //}
    void Start()
    {
        //textEntries = new List<string>();
        //ActivateChat(new List<string>() { "Prime the textbox", "iterate"}, null);  // TODO: hopefully asset text system will fix textbox problem
        //ProgressChat(); ProgressChat();
        DeactivateChat();
    }

    void Update()
    {
        if (chatOpen && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            Debug.Log("Update");
            ProgressChat();
        }
        //else if (chatOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.J)))
        //{
        //    DeactivateChat();
        //}
    }

    public void DeactivateChat()
    {
        chatBox.SetActive(false);
        speakerImg.SetActive(false);
        dialogue = new List<string>();
        chatOpen = false;
        ui.ActivateUI();
    }

    public void ActivateChat(List<string> newDialogue, Sprite speakerPic)
    {
        ui.DeactivateUI();
        chatBox.SetActive(true);
        textEntries = new List<string>();
        dialogue = newDialogue;
        speakerImg.SetActive(true);
        speakerImg.GetComponent<Image>().sprite = speakerPic;
        chatOpen = true;
        currDialogue = 0;
        ProgressChat();
    }

    private void ProgressChat()
    {
        currDialogue++;
        Debug.Log("curr dialogue " + currDialogue);
        if (currDialogue > dialogue.Count) DeactivateChat();
        else AddText(dialogue[currDialogue - 1]);
    }
    

    public void AddText(string newEntry)
    {
        textEntries.Add(newEntry);
        Debug.Log("add " + newEntry);
        UpdateTextLog();
    }

    private void UpdateTextLog()
    {
        //textContent.text = "";
        for (int i = 0; i < textEntries.Count; i++)
        {
            Debug.Log("update text " + textEntries[i]);
            if (i > 0) textContent.text += "\n";
            textContent.text += textEntries[i];
            Debug.Log("text content" + textContent.text);
            textContent.ForceMeshUpdate(); chatBox.SetActive(false); chatBox.SetActive(true);
        }
        SetToBottom();
    }

    public void SetToBottom()
    {
        //var scrollRectPos = gameObject.GetComponent<ScrollRect>().normalizedPosition;
        textContent.transform.localPosition = new Vector2(textContent.transform.localPosition.x, textContent.GetComponent<RectTransform>().rect.height);
    }
}

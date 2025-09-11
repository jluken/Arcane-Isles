using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MenuScreen // TODO: handle more complex behavior like trees and updating char pic with dialogueTree system
{
    public static DialogueBox Instance { get; private set; }

    public GameObject chatBox;
    public TMP_Text textContent;
    public GameObject speakerImg;
    private UIController ui;

    public List<string> textEntries;

    private bool chatOpen;

    private List<string> dialogue;
    private int currDialogue;  //TODO: this is very much a placeholder and will be replaced with tree asset

    //private void Awake()
    //{
    //    ActivateChat(new List<string>() { "Prime the textbox", "iterate" }, null);
    //}
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ui = UIController.Instance;
        //ActivateChat(new List<string>() { "Prime the textbox", "iterate"}, null);  // TODO: hopefully asset text system will fix textbox problem
        //ProgressChat(); ProgressChat();
        //DeactivateChat();
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

    public override void DeactivateMenu()
    {
        chatBox.SetActive(false);
        speakerImg.SetActive(false);
        dialogue = new List<string>();
        chatOpen = false;
    }

    public void SetSpeaker(List<string> newDialogue, Sprite speakerPic)
    {
        speakerImg.GetComponent<Image>().sprite = speakerPic;
        dialogue = newDialogue;
    }

    public override void ActivateMenu()
    {
        chatBox.SetActive(true);
        textEntries = new List<string>();
        speakerImg.SetActive(true);
        chatOpen = true;
        currDialogue = 0;
        ProgressChat();
    }

    public override bool IsActive()
    {
        return chatOpen;
    }

    public override bool overlay => false;

    private void ProgressChat()
    {
        currDialogue++;
        Debug.Log("curr dialogue " + currDialogue);
        if (currDialogue > dialogue.Count) ui.ActivateDefaultScreen();
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

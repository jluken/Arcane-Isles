using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextLog : MonoBehaviour
{
    public TMP_Text textContent;

    private List<string> textEntries;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textEntries = new List<string>();
        UpdateTextLog();
    }

    public void AddText(string newEntry)
    {
        textEntries.Add(newEntry);
        UpdateTextLog();
    }

    private void UpdateTextLog()
    {
        textContent.text = "";
        for (int i = 0; i < textEntries.Count; i++) {
            if (i > 0) textContent.text += "\n";
            textContent.text += textEntries[i];
        }
        SetToBottom();
    }

    public void SetToBottom()
    {
        var scrollRectPos = gameObject.GetComponent<ScrollRect>().normalizedPosition;
        //Debug.Log("text max height: " + textContent.maxHeight);
        //Debug.Log("rect height: " + textContent.GetComponent<RectTransform>().rect.height);
        //Debug.Log("text flex height: " + textContent.flexibleHeight);
        //gameObject.GetComponent<ScrollRect>().normalizedPosition = new Vector2(scrollRectPos.x, 0);
        textContent.transform.localPosition = new Vector2(textContent.transform.localPosition.x, textContent.GetComponent<RectTransform>().rect.height);
        //gameObject.GetComponent<ScrollRect>().normalizedPosition = new Vector2(gameObject.GetComponent<ScrollRect>().normalizedPosition.x, gameObject.GetComponent<ScrollRect>().normalizedPosition.y);
    }
}

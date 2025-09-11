using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class SelectionController : MonoBehaviour
{
    public static SelectionController Instance { get; private set; }

    public delegate void SelectEvent();
    public event SelectEvent selectEvent;

    private void Awake()
    {
        Instance = this;
    }

    public void NewSelection()
    {
        selectEvent?.Invoke();  // Monitored by Selectable items to deselect when triggered
    }

}

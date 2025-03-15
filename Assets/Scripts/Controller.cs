using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Controller : MonoBehaviour
{

    public delegate void SelectEvent(); // TODO: do I need multiple void delegates?
    public event SelectEvent selectEvent;


    public void NewSelection()
    {
        selectEvent?.Invoke();
    }

}

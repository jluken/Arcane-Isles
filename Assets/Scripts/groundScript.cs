using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class groundScript : MonoBehaviour 
{

    public bool accessible { get; private set; }

    public void Start()
    {
        accessible = true;
    }
}

using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class groundScript : MonoBehaviour 
{

    public bool accessible { get; private set; } // TODO: just replace this all with layer?

    public void Start()
    {
        accessible = true;
    }
}

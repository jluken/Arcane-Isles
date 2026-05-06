using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class groundScript : MonoBehaviour 
{

    public bool accessible { get; private set; }  // TODO: will need to toggle on/off when area is "discovered"

    public void Start()
    {
        accessible = true;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SlideshowImages", menuName = "Scriptable Objects/SlideshowImages")]
public class SlideshowImages : ScriptableObject
{
    public List<Sprite> slides;
}

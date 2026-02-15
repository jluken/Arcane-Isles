using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBox : MonoBehaviour
{
    public SkillBar skillBar;
    public int boxId;

    private static Sprite naSprite;
    private Sprite filledSprite;
    private Sprite emptySprite;
    private Sprite availableSprite;
    private Sprite bonusSprite;
    private Sprite penaltySprite;

    public enum BoxState
    {
        na,
        filled,
        tempFilled,
        empty,
        available,
        tempAvailable,
        newBoxAvailable,
        bonus,
        penalty
    }

    private Dictionary<BoxState, Sprite> boxStateImage = new Dictionary<BoxState, Sprite>();

    public BoxState boxState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //Debug.Log("loading box sprites");
        boxStateImage[BoxState.na] = Resources.Load<Sprite>("Sprites/naBoxSprite");  // TODO: possibly store this as an object of menu
        boxStateImage[BoxState.filled] = Resources.Load<Sprite>("Sprites/filledBoxSprite");  // TODO: dedicated sprite loader with error handling
        boxStateImage[BoxState.tempFilled] = Resources.Load<Sprite>("Sprites/tempFilledBoxSprite");
        boxStateImage[BoxState.empty] = Resources.Load<Sprite>("Sprites/emptyBoxSprite");
        boxStateImage[BoxState.available] = Resources.Load<Sprite>("Sprites/availableBoxSprite");
        boxStateImage[BoxState.tempAvailable] = Resources.Load<Sprite>("Sprites/tempAvailableBoxSprite");
        boxStateImage[BoxState.newBoxAvailable] = Resources.Load<Sprite>("Sprites/newBoxAvailableBoxSprite");
        boxStateImage[BoxState.bonus] = Resources.Load<Sprite>("Sprites/bonusBoxSprite");
        boxStateImage[BoxState.penalty] = Resources.Load<Sprite>("Sprites/penaltyBoxSprite");
    }

    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(boxClick);
    }

    public void Populate(BoxState state)
    {
        boxState = state;
        GetComponent<Button>().image.sprite = boxStateImage[state];
    }

    public void OnDeck()
    {
        if(boxState == BoxState.empty) boxState = BoxState.available;
        else if (boxState == BoxState.na) boxState = BoxState.newBoxAvailable;
    }
    public void OffDeck()
    {
        if (boxState == BoxState.available) boxState = BoxState.empty;
        else if (boxState == BoxState.newBoxAvailable) boxState = BoxState.na;
    }

    public void boxClick()
    {
        Debug.Log("box click");
        skillBar.BoxClicked(boxId);
    }
}

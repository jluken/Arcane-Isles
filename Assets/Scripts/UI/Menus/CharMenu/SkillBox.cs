using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBox : MonoBehaviour
{
    public SkillBar skillBar;
    public int boxId;

    public enum BoxState
    {
        na,
        filled,
        tempFilled,
        empty,
        available,
        newBoxAvailable,
        newBoxFilled,
        bonus,
        penalty
    }

    private Dictionary<BoxState, Sprite> boxStateImage = new Dictionary<BoxState, Sprite>();

    public BoxState boxState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //Debug.Log("loading box sprites");
        boxStateImage[BoxState.na] = CharacterMenu.Instance.naSprite;
        boxStateImage[BoxState.filled] = CharacterMenu.Instance.filledSprite;
        boxStateImage[BoxState.tempFilled] = CharacterMenu.Instance.tempFilledSprite;
        boxStateImage[BoxState.empty] = CharacterMenu.Instance.emptySprite;
        boxStateImage[BoxState.available] = CharacterMenu.Instance.availableSprite;
        boxStateImage[BoxState.newBoxAvailable] = CharacterMenu.Instance.extraAvailableSprite;
        boxStateImage[BoxState.newBoxFilled] = CharacterMenu.Instance.extraTempFilledSprite;
        boxStateImage[BoxState.bonus] = CharacterMenu.Instance.bonusSprite;
        boxStateImage[BoxState.penalty] = CharacterMenu.Instance.penaltySprite;
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

    //public void OnDeck()
    //{
    //    if(boxState == BoxState.empty) boxState = BoxState.available;
    //    else if (boxState == BoxState.na) boxState = BoxState.newBoxAvailable;
    //}
    //public void OffDeck()
    //{
    //    if (boxState == BoxState.available) boxState = BoxState.empty;
    //    else if (boxState == BoxState.newBoxAvailable) boxState = BoxState.na;
    //}

    public void boxClick()
    {
        Debug.Log("box click");
        skillBar.BoxClicked(boxId);
    }
}

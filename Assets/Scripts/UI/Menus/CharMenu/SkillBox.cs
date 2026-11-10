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
        newBoxSet,
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
        boxStateImage[BoxState.newBoxSet] = CharacterMenu.Instance.extraTempSetSprite;
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

    public int SetClickedBox(bool pointsLeft)
    {
        // Returns the amount of points spent from click
        if (boxState == BoxState.available) {Populate(BoxState.tempFilled); return 1; }
        else if (boxState == BoxState.tempFilled) {Populate(BoxState.available); return -1; }
        else if (boxState == BoxState.newBoxAvailable) {Populate(BoxState.newBoxSet); return 1; }
        else if (boxState == BoxState.newBoxSet && pointsLeft) {Populate(BoxState.newBoxFilled); return 1; }
        else if (boxState == BoxState.newBoxSet && !pointsLeft) {Populate(BoxState.newBoxAvailable); return -1; }
        else if (boxState == BoxState.newBoxFilled) {Populate(BoxState.newBoxAvailable); return -2; }
        else { Debug.LogError("Invalid Box transition"); return 0; }
    }

    public void SetNextBox(bool relinquishedBox)
    {
        if (boxState == BoxState.na && !relinquishedBox) Populate(BoxState.newBoxAvailable);
        else if (boxState == BoxState.newBoxAvailable) Populate(BoxState.na);
        else if (boxState == BoxState.empty && !relinquishedBox) Populate(BoxState.available);
        else if (boxState == BoxState.available) Populate(BoxState.empty);
        else Debug.LogError("Invalid Box transition");
    }

    public void boxClick()
    {
        Debug.Log("box click");
        skillBar.BoxClicked(boxId);
    }
}

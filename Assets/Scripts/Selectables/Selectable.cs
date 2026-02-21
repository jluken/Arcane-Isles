using NUnit.Framework.Internal;
using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Selectable : MonoBehaviour
{
    public Vector3 standPoint;  // TODO: possibly reevaluate whether this is best practice. Old 2d games like BG1 use it, but not modern games like BG3
    //private SelectionController selectionController;

    protected Interaction interactAction;

    private ItemSelectMenu selectMenu;

    public string description;
    private IEnumerator displayRoutine;
    private GameObject itemPopUpPrefab;
    private GameObject itemPopUp;

    //private void Awake()
    //{

    //}

    // Start is called before the first frame update
    //public List<ActionName> actionNames;

    //public enum ActionName // Used for optional actions
    //{
    //    Select,
    //    Inspect,
    //    Go_Here,
    //    Talk,
    //    Trade,
    //    Recruit,
    //    Stay,
    //    Follow,
    //    Move_To,
    //    Open_Container,
    //    Open_Door,
    //    Go_To,
    //    Attack
    //};
    public SelectionData inspectSelection;
    public SelectionData goHere;
    public SelectionData combatMovement;
    public SelectionData openInventory;

    //public Dictionary<ActionName, Action> actionMapping;

    public virtual void Awake()
    {
        goHere = new SelectionData(this)
        {
            actionName = "Go Here",
            setSelect = true
        };
        inspectSelection = new SelectionData(this)
        {
            actionName = "Inspect",
            immediateAction = Inspect
        };
        openInventory = new SelectionData(this)
        {
            actionName = "Open",
            setSelect = true,
            interaction = new Open()
        };
        combatMovement = new SelectionData(this)
        {
            actionName = "Go Here",
            setSelect = true,
            //immediateAction = CombatMove
        };
    }

    public virtual void Start()
    {
        SelectionController.Instance.deselectEvent += UnsetInteraction;
        itemPopUpPrefab = Resources.Load<GameObject>("Prefabs/ItemPopup");
        selectMenu = ItemSelectMenu.Instance;

        displayRoutine = DisplayText();
    }

    public virtual void Interact(NPC npc)
    {
        Debug.Log("Interact: " + this.name + interactAction);
        interactAction?.Interact(npc, this);
        SelectionController.Instance.Deselect();
        UnsetInteraction();
        //isActive = true;
    }

    public void HoverDisplay()
    {
        //Debug.Log("Hover over object " + gameObject.name);
        //TODO: highlight object - possibly use asset if can't find way to "outline" in 3D
    }

    public void Select()
    {
        SelectionController.Instance.Select(this);
    }

    public void SetInteractAction(Interaction activeAct)
    {
        interactAction = activeAct;
    }

    public void UnsetInteraction()
    {
        interactAction = null;
    }

    public virtual List<SelectionData> Actions()
    {
        var acts = new List<SelectionData>() { goHere, inspectSelection};
        return acts;
    }

    //public virtual List<SelectionData> CombatActions()
    //{
    //    //Returns the actions accessible during combat
    //    return Actions();
    //}

    public void Inspect()
    {
        //dialogueBox.AddText(description); // TODO: what sort of dialogue box should be persistant, separate from talking box
        //StopCoroutine(displayRoutine);
        //displayRoutine = DisplayText();
        //StartCoroutine(displayRoutine);
        DialogueInterface.Instance.DescriptionBark(this);
    }

    //public void CombatMove()
    //{
    //    PartyController.Instance.GoTo(gameObject.transform.position);
    //    //CombatManager.Instance.UseCombatAbility(this, CombatManager.CombatActionType.Run);
    //}

    IEnumerator DisplayText()
    {
        var height = gameObject.GetComponent<MeshRenderer>().bounds.max.y;
        Destroy(itemPopUp);
        itemPopUp = Instantiate(itemPopUpPrefab, transform.position, itemPopUpPrefab.transform.rotation);
        itemPopUp.transform.SetParent(transform);
        itemPopUp.transform.localPosition = new Vector3(0, (height / 2) + 2, 0);
        itemPopUp.GetComponent<TMP_Text>().text = description;
        itemPopUp.SetActive(true);
        yield return new WaitForSeconds(3);
        Destroy(itemPopUp);// TODO: fade out animation?
    }
}

public abstract class Interaction  // TODO: possibly have every interaction carry an AP cost?
{
    public abstract void Interact(NPC npc, Selectable interactable);
}

public class Open : Interaction
{
    public override void Interact(NPC npc, Selectable interactable)
    {
        if (interactable.GetComponent<Container>() == null  && interactable.GetComponent<NPC>() == null) { Debug.LogError("Can only open objects with inventories"); }
        var container = interactable.GetComponent<Container>();
        if (container == null) UIController.Instance.ActivateContainerScreen(interactable.GetComponent<NPC>().inventory);
        else UIController.Instance.ActivateContainerScreen(container.inventory);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Selectable : MonoBehaviour
{
    private bool selected;
    private bool isActive;
    //private bool reached;
    public Vector3 standPoint;  // TODO: possibly reevaluate whether this is best practice. Old 2d games like BG1 use it, but not modern games like BG3
    //private SelectionController selectionController;

    private Action interactAction;

    private ItemSelectMenu selectMenu;

    public string description;
    private IEnumerator displayRoutine;
    private GameObject itemPopUpPrefab;
    private GameObject itemPopUp;
    private DialogueBox dialogueBox;

    //private void Awake()
    //{
        
    //}

    // Start is called before the first frame update
    public virtual void Start()
    {
        isActive = false;
        //selectionController = SelectionController.Instance;
        SelectionController.Instance.selectEvent += Deselect;
        itemPopUpPrefab = Resources.Load<GameObject>("Prefabs/ItemPopup");
        selectMenu = ItemSelectMenu.Instance;
        dialogueBox = DialogueBox.Instance;

        //if (textLog != null) textLogText = textLog.GetComponent<TextLog>();
        displayRoutine = DisplayText();
        selected = false;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Barrier");
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, ~layerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject(-1) && hit.transform.gameObject == gameObject)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Actions()[0].Item2.Invoke();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("select menu: " + selectMenu);
                UIController.Instance.ActivateItemSelect(Input.mousePosition, Actions());
            }

            HoverDisplay();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PartyController.Instance.leader.charObject && !IsActive() && selected)
        {
            RaycastHit hit;
            var rayDirection = other.gameObject.transform.position - transform.position;
            if (Physics.Raycast(transform.position, rayDirection, out hit))
            {
                if (hit.transform == other.gameObject.transform)
                {  // Line of sight between player and target
                    Interact();
                    PartyController.Instance.leader.charObject.GetComponent<MoveToClick>().StopMoving();
                }

            }
        }
    }

    public virtual void Interact()
    {
        interactAction?.Invoke();
        Deselect();
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    public void HoverDisplay()
    {
        //Debug.Log("Hover over object " + gameObject.name);
        //TODO: highlight object - possibly use asset if can't find way to "outline" in 3D
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void SetTarget()
    {
        SelectionController.Instance.NewSelection();
        selected = true;
        PartyController.Instance.SetPartyDestination(transform.position);  // TODO: if standpoint, set here with absolute values
    }

    public void SetInteractAction(Action activeAct)
    {
        interactAction = activeAct;
    }

    public void Deselect()
    {
        interactAction = null;
        selected = false;
        Deactivate();
    }

    public virtual List<(string, Action)> Actions()
    {
        var acts = new List<(string, Action)>();
        acts.Add(("Go Here", SetTarget));
        // Derived classes can have more options
        
        return acts;
    }

    public void Inspect()
    {

        //dialogueBox.AddText(description); // TODO: what sort of dialogue box should be persistant, separate from talking box
        StopCoroutine(displayRoutine);
        displayRoutine = DisplayText();
        StartCoroutine(displayRoutine);
    }

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

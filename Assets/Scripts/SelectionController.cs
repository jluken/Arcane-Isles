using PixelCrushers.DialogueSystem;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class SelectionData
{
    public string actionName;
    public Selectable selectable;
    public bool setSelect;
    public Action immediateAction;
    public Interaction interaction;

    public SelectionData(Selectable selectable)
    {
        this.selectable = selectable;
    }
}

public class SelectionController : MonoBehaviour
{
    public static SelectionController Instance { get; private set; }

    public event Action deselectEvent;

    public Selectable selectedItem;
    public Vector3 lastHitPoint { get; private set; }
    public bool playerUnderControl;

    private bool talking;

    private void Awake()
    {
        Instance = this;
        playerUnderControl = true;
    }

    public void Start()
    {
        DialogueManager.instance.conversationStarted += (sender) => { talking = true; };  // TODO: wrap this up in the controller for general UI
        DialogueManager.instance.conversationEnded += (sender) => { talking = false; };
    }

    void Update()
    {
        if (!playerUnderControl || CombatManager.Instance.inAction || talking) return;
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Barrier"); // TODO: maybe detect selectable specifically instead of ignoring barrier
        bool objectPoint = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, ~layerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject(-1);
        if (objectPoint && hit.transform.gameObject.GetComponent<groundScript>() != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Deselect();
                PartyController.Instance.GoTo(hit.point);
            }
        }
        else if (objectPoint && hit.transform.gameObject.GetComponent<Selectable>() != null)
        {
            var pointedSelectable = hit.transform.gameObject.GetComponent<Selectable>();
            var actions = CombatManager.Instance.combatActive ? pointedSelectable.CombatActions() : pointedSelectable.Actions();
            if (Input.GetMouseButtonDown(0))
            {
                lastHitPoint = hit.point; // TODO: used anymore?
                InitiateSelection(actions[0]);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (CombatManager.Instance.combatActive) CombatManager.Instance.UnsetAction();
                lastHitPoint = hit.point;
                UIController.Instance.ActivateItemSelect(Input.mousePosition, actions);
            }

            pointedSelectable.HoverDisplay();
        }
    }

    public void InitiateSelection(SelectionData selectionData)
    {
        if (selectionData.immediateAction != null) selectionData.immediateAction.Invoke();
        if (selectionData.selectable != null)
        {
            if (selectionData.setSelect)
            {
                Select(selectionData.selectable); 
                PartyController.Instance.GoTo(selectionData.selectable.transform.position);
            }
            if (selectionData.interaction != null) selectionData.selectable.SetInteractAction(selectionData.interaction);
        }
    }

    public void Select(Selectable selectable)
    {
        Deselect();
        selectedItem = selectable;
        Debug.Log("Select: " + selectable.name);
        //PartyController.Instance.GoTo(selectable.transform.position);  // TODO: if selectable standpoint, set here with absolute values
    }

    public void Deselect()
    {
        selectedItem = null;
        Debug.Log("Deselect: " + this.name);
        deselectEvent?.Invoke();  // Monitored by Selectable items to deselect when triggered
    }

    public bool IsSelected(GameObject obj)
    {
        return selectedItem != null && obj == selectedItem.gameObject;
    }

}

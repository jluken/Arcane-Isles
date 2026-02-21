using PixelCrushers.DialogueSystem;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.InputSystem.DefaultInputActions;

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
    public bool playerUnderControl;

    private GameObject pointedObject;
    private Vector3 pointSpot;

    private void Awake()
    {
        Instance = this;
        playerUnderControl = true;
    }

    public void Start()
    {
        InputActionMap uiActions = InputSystem.actions.FindActionMap("UI");
        uiActions.FindAction("Click").performed += (sender) => HandleClick(true);
        uiActions.FindAction("RightClick").performed += (sender) => HandleClick(false);
    }

    void Update()
    {
        if (!playerUnderControl || CombatManager.Instance.inAction || UIController.Instance.PauseTime()) return;
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Barrier"); // TODO: maybe detect selectable specifically instead of ignoring barrier

        bool objectPoint = Physics.Raycast(Camera.main.ScreenPointToRay(MousePosition()), out hit, 100, ~layerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject(-1);
        pointedObject = objectPoint ? hit.transform.gameObject : null;
        pointSpot = hit.point;
        pointedObject?.GetComponent<Selectable>()?.HoverDisplay();
    }

    private void HandleClick(bool leftClick)
    {
        if (pointedObject?.GetComponent<groundScript>() != null)
        {
            if (leftClick)
            {
                Deselect();
                PartyController.Instance.GoTo(pointSpot);
            }
        }
        else if (pointedObject?.GetComponent<Selectable>() != null)
        {
            var pointedSelectable = pointedObject.GetComponent<Selectable>();
            var actions = pointedSelectable.Actions();
            if (leftClick)
            {
                InitiateSelection(actions[0]);
            }
            else
            {
                if (CombatManager.Instance.combatActive) CombatManager.Instance.UnsetAction();
                UIController.Instance.ActivateItemSelect(MousePosition(), actions);
            }
        }
    }

    public Vector2 MousePosition() => Mouse.current is not null ? Mouse.current.position.value : new Vector2(Screen.width / 2, Screen.height / 2);

    public Vector2 MouseScroll() => Mouse.current is not null ? Mouse.current.scroll.value : new Vector2();

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

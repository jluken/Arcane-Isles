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
        if (UIController.Instance.PauseTime()) return;
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Barrier");

        bool objectPoint = Physics.Raycast(Camera.main.ScreenPointToRay(MousePosition()), out hit, 100, ~layerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject();
        
        if (objectPoint)
        {
            if (pointedObject != hit.transform.gameObject)
            {
                if (pointedObject != null && pointedObject.GetComponent<Selectable>() != null) pointedObject.GetComponent<Selectable>().EndHover();
                pointedObject = hit.transform.gameObject;
                //if (pointedObject.GetComponent<Selectable>() != null) pointedObject.GetComponent<Selectable>().StartHover();
            }
            pointSpot = hit.point;
        }
        else
        {
            if (pointedObject != null && pointedObject.GetComponent<Selectable>() != null) pointedObject.GetComponent<Selectable>().EndHover();
            pointedObject = null;        
        }
        HandleHover();
    }

    private void HandleClick(bool leftClick)
    {
        if (UIController.Instance.PauseTime() || (SceneLoader.Instance.GetLevel() != null && SceneLoader.Instance.GetLevel().InsideBlockedRegion(pointSpot))) return;
        if (pointedObject != null && pointedObject.GetComponent<groundScript>() != null)
        {
            if (leftClick && PartyController.Instance.selectedPartyMember == PartyController.Instance.activePartyMember)
            {
                if (CombatManager.Instance.currentAction == null)
                {
                    Deselect();
                    PartyController.Instance.GoTo(pointSpot);
                }
                else CombatManager.Instance.TargetPoint(pointSpot);
            }
        }
        else if (pointedObject != null && pointedObject.GetComponent<Selectable>() != null)
        {
            var pointedSelectable = pointedObject.GetComponent<Selectable>();
            var invisible = SceneLoader.Instance.GetLevel().InsideInvisibleRegion(pointedObject.transform.position);
            var actions = pointedSelectable.Actions();
            if (leftClick)
            {
                if (invisible) PartyController.Instance.GoTo(pointedObject.GetComponent<Selectable>());
                else InitiateSelection(actions[0]);
            }
            else if (!invisible)
            {
                if (CombatManager.Instance.combatActive) CombatManager.Instance.UnsetAction();
                UIController.Instance.ActivateItemSelect(MousePosition(), actions);
            }
        }
    }

    private void HandleHover() // TODO: cleanup
    {
        if (UIController.Instance.PauseTime() || (SceneLoader.Instance.GetLevel() != null && SceneLoader.Instance.GetLevel().InsideBlockedRegion(pointSpot))) return;
        if (pointedObject != null && pointedObject.GetComponent<Selectable>() != null) pointedObject.GetComponent<Selectable>().StartHover();
        if (CombatManager.Instance.combatActive && PartyController.Instance.selectedPartyMember == PartyController.Instance.activePartyMember)
        {
            if (pointedObject != null && (pointedObject.GetComponent<groundScript>() != null || pointedObject.GetComponent<Selectable>() != null))
            {
                var mainChar = PartyController.Instance.selectedPartyMember;
                //if ((CombatManager.Instance.currentAction == null || CombatManager.Instance.currentAction == CombatManager.Instance.defaultRun) && !mainChar.mover.IsMoving())
                //{
                //    if (pointedObject.GetComponent<groundScript>() != null) mainChar.mover.DrawTo(pointSpot);
                //    else mainChar.mover.DrawTo(pointedObject.GetComponent<Selectable>());
                //}
                if (pointedObject.GetComponent<groundScript>() != null) CombatManager.Instance.PrepTargetPoint(pointSpot);
                else CombatManager.Instance.PrepAttackTarget(pointedObject.GetComponent<Selectable>());
                //else CombatManager.Instance.TargetPoint(pointSpot);
            }
            else CombatManager.Instance.UpdateCombatDisplay(null);
        }
        else CombatManager.Instance.UpdateCombatDisplay(null);
        //if (pointedObject != null && pointedObject.GetComponent<groundScript>() != null)
        //{
        //    if (PartyController.Instance.selectedPartyMember == PartyController.Instance.activePartyMember)
        //    {
        //        var mainChar = PartyController.Instance.selectedPartyMember;
        //        if (CombatManager.Instance.currentAction == null && !mainChar.mover.IsMoving())
        //        {
        //            mainChar.mover.DrawTo(pointSpot);
        //        }
        //        //else CombatManager.Instance.TargetPoint(pointSpot);
        //    }
        //}
        //else if (pointedObject != null && pointedObject.GetComponent<Selectable>() != null)
        //{
        //    if (PartyController.Instance.selectedPartyMember == PartyController.Instance.activePartyMember)
        //    {
        //        var mainChar = PartyController.Instance.selectedPartyMember;
        //        if (CombatManager.Instance.currentAction == null && !mainChar.mover.IsMoving())
        //        {
        //            mainChar.mover.DrawTo(pointedObject.GetComponent<Selectable>());
        //        }
        //        //else CombatManager.Instance.TargetPoint(pointSpot);
        //    }
        //}
    }

    public Vector2 MousePosition() => Mouse.current is not null ? Mouse.current.position.value : new Vector2(Screen.width / 2, Screen.height / 2);

    public Vector2 MouseScroll() => Mouse.current is not null ? Mouse.current.scroll.value : new Vector2();

    public void InitiateSelection(SelectionData selectionData)
    {
        if (CombatManager.Instance.InAction() && !CombatManager.Instance.Running()) return;  // Can't select while combat busy
        if (selectionData.immediateAction != null) selectionData.immediateAction.Invoke();
        if (selectionData.selectable != null)
        {
            if (selectionData.setSelect)
            {
                Select(selectionData.selectable); 
                PartyController.Instance.GoTo(selectionData.selectable);
            }
            if (selectionData.interaction != null) selectionData.selectable.SetInteractAction(selectionData.interaction);
        }
    }

    public void Select(Selectable selectable)
    {
        Deselect();
        selectedItem = selectable;
    }

    public void Deselect()
    {
        selectedItem = null;
        deselectEvent?.Invoke();  // Monitored by Selectable items to deselect when triggered
    }

    public bool IsSelected(GameObject obj)
    {
        return selectedItem != null && obj == selectedItem.gameObject;
    }

}

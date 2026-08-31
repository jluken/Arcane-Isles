using System;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public static EventHandler Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public delegate void CharEvent(Character npc);
    public event CharEvent deathEvent;

    public void TriggerDeathEvent(Character npc)
    {
        deathEvent?.Invoke(npc);
    }

    public delegate void CharMovementEvent(Character npc, string movement);
    public event CharMovementEvent charMovement;

    public void TriggerMovementEvent(Character npc, string movement)
    {
        charMovement?.Invoke(npc, movement);
    }

    public event Action inventoryUpdate;
    public void TriggerInventoryUpdate() { inventoryUpdate?.Invoke(); }

    public delegate void SelectableEvent(Selectable obj);
    public event SelectableEvent hoverObj;
    public void TriggerObjHover(Selectable obj)
    {
        hoverObj?.Invoke(obj);
    }
    public event SelectableEvent unhoverObj;
    public void TriggerObjUnhover(Selectable obj)
    {
        unhoverObj?.Invoke(obj);
    }


}

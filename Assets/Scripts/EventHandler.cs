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

    public event Action inventoryUpdate;
    public void TriggerInventoryUpdate() { inventoryUpdate?.Invoke(); }

}

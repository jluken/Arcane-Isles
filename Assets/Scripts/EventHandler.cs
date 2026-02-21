using System;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public static EventHandler Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public delegate void CharEvent(NPC npc);
    public event CharEvent deathEvent;

    public void TriggerDeathEvent(NPC npc)
    {
        deathEvent?.Invoke(npc);
    }

    public event Action inventoryUpdate;
    public void TriggerInventoryUpdate() { inventoryUpdate?.Invoke(); }

}

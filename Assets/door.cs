using Unity.AI.Navigation;
using UnityEngine;

public class doorway : Selectable
{

    public bool open = false;
    public GameObject door;  // TODO: with actual assets, door will be one "object" and can have separate invisible barrier tied to it

    private NavMeshSurface surface;

    private void Start()
    {
        surface = GameObject.Find("Floor").GetComponent<NavMeshSurface>();
        door.SetActive(!open);
        base.Start();
    }

    public override void Activate()
    {
        Debug.Log("activate door");
        open = !open;
        door.SetActive(!open);
        //surface.BuildNavMesh();
        base.Activate();
    }
}

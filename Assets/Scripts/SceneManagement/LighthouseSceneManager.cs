using UnityEngine;

public class LighthouseSceneManager : SceneObjectManager
{
    // Can put customly tracked NPCs/items here that are referenced in the custom awake
    // Can also put flags for specific save checks if they've already been applied so they don't do it twice

    protected override void Awake()
    {
        base.Awake();

        // Check save data for custom behavior with custom objects above
    }

}

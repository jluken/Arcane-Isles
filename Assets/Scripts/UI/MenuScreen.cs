using UnityEngine;

public abstract class MenuScreen : MonoBehaviour
{
    
    public abstract void ActivateMenu();

    public abstract void DeactivateMenu();

    public abstract bool IsActive();

    public abstract bool overlay { get; }
}

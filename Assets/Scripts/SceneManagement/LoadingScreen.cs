using UnityEngine;

public class LoadingScreen : MenuScreen
{
    public GameObject loadingScreen;

    private bool active;
    public override void ActivateMenu()
    {
        loadingScreen.SetActive(true);
        active = true;
    }

    public override void DeactivateMenu()
    {
        loadingScreen.SetActive(false);
        active = false;
    }

    public override bool IsActive()
    {
        return active;
    }

    public override bool overlay => false;
}

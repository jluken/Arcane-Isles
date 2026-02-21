using UnityEngine;

public class LoadingScreen : MenuScreen
{
    public GameObject loadingScreen;
    public static LoadingScreen Instance;

    private bool active;

    public void Awake()
    {
        Instance = this;
    }
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
}

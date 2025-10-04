using UnityEngine;

public class MainMenu : MenuScreen
{
    public GameObject mainMenu;  //TODO: how much of this belongs in the UI Controller now that it's always on?

    private bool active;
    public override void ActivateMenu()
    {
        mainMenu.SetActive(true);
        active = true;
    }

    public override void DeactivateMenu()
    {
        mainMenu.SetActive(false);
        active = false;
    }

    public override bool IsActive()
    {
        return active;
    }

    public override bool overlay => false;
}

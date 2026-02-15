using UnityEngine;

public class MainMenu : MenuScreen
{
    public GameObject mainMenu;  //TODO: how much of this belongs in the UI Controller now that it's always on?

    private bool active;

    public void Settings()  // TODO: when open settings menu, should still be able to exit out if in main menu mode (treat as overlay even if full screen)
    {
        UIController.Instance.ActivateSettings();
    }

    public void Load() // TODO: deal with duplicate with main menu UI
    {
        SceneLoader.Instance.LoadFromData(SaveSystem.LoadGame());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

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

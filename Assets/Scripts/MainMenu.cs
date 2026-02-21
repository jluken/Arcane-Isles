using UnityEngine;

public class MainMenu : MenuScreen
{
    public static MainMenu Instance;

    public GameObject mainMenu;

    private bool active;

    public void Awake()
    {
        Instance = this;
    }

    public void Settings()
    {
        UIController.Instance.ActivateSettings();
    }

    public void Load() // TODO: Implement proper save menu/system
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
}

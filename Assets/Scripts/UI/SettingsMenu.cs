using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MenuScreen
{
    public GameObject settingsMenu;
    public Button minus;
    public Button plus;

    public TMP_Text settingVal;

    private bool active;

    public void Start()
    {
        //minus.onClick.AddListener(() => ClickChange(-1));
        //plus.onClick.AddListener(() => ClickChange(1));
    }
    public override void ActivateMenu()
    {
        settingsMenu.SetActive(true);
        settingVal.text = PlayerPrefs.GetInt("settingVal").ToString();
        active = true;
    }

    public override void DeactivateMenu()
    {
        settingsMenu.SetActive(false);
        active = false;
    }

    public override bool IsActive()
    {
        return active;
    }

    public override bool overlay => true;

    public void ClickChange(int valChange)  // TODO: store all menu button logic in menu script (look at pause menu for examples)
    {
        PlayerPrefs.SetInt("settingVal", PlayerPrefs.GetInt("settingVal") + valChange);
        settingVal.text = PlayerPrefs.GetInt("settingVal").ToString();
    }
}

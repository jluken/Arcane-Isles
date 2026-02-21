using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MenuScreen
{
    public GameObject settingsMenu;

    public static SettingsMenu Instance;

    public Slider music;
    private static string musicName = "musicVolume";
    private float defaultVol = 100f;
    public Slider sfx;
    private static string sfxName = "sfxVolume";
    public Slider ambient;
    private static string ambientName = "ambientVolume";

    public TMP_Dropdown resolution;
    private static string resolutionName = "screenResolution";
    private static string[] resOptions = {"720", "1080", "1440"};
    private int defaultResolution = 1;

    public Toggle colorblind;
    private static string colorblindName = "colorBlindToggle";
    private int defaultColorblind = 0;
    public TMP_Dropdown fontType;
    private static string fontTypeName = "fontType";
    private static string[] fontOptions = { "standard", "noCursive", "dyslexic" };
    private int defaultFont = 0;



    public Button minus;
    public Button plus;

    public TMP_Text settingVal;

    private bool active;

    public void Awake()
    {
        Instance = this;
        //minus.onClick.AddListener(() => ClickChange(-1));
        //plus.onClick.AddListener(() => ClickChange(1));
        if (!PlayerPrefs.HasKey(musicName)) PlayerPrefs.SetFloat(musicName, defaultVol); // TODO: these setting should be set outside of menu on game startup
        if (!PlayerPrefs.HasKey(sfxName)) PlayerPrefs.SetFloat(sfxName, defaultVol);
        if (!PlayerPrefs.HasKey(ambientName)) PlayerPrefs.SetFloat(ambientName, defaultVol);

        if (!PlayerPrefs.HasKey(resolutionName)) PlayerPrefs.SetString(resolutionName, resOptions[defaultResolution]);

        if (!PlayerPrefs.HasKey(colorblindName)) PlayerPrefs.SetInt(resolutionName, defaultColorblind);
        if (!PlayerPrefs.HasKey(fontTypeName)) PlayerPrefs.SetString(resolutionName, fontOptions[defaultFont]);
    }
    public override void ActivateMenu()
    {
        settingsMenu.SetActive(true);
        UpdateValues();
        settingVal.text = PlayerPrefs.GetInt("settingVal").ToString();
        active = true;
    }

    private void UpdateValues()
    {
        music.value = PlayerPrefs.GetFloat(musicName);
        sfx.value = PlayerPrefs.GetFloat(sfxName);
        ambient.value = PlayerPrefs.GetFloat(ambientName);

        resolution.value = Array.IndexOf(resOptions, PlayerPrefs.GetString(resolutionName));

        colorblind.isOn = PlayerPrefs.GetInt(colorblindName) == 1;
        fontType.value = Array.IndexOf(fontOptions, PlayerPrefs.GetString(fontTypeName));
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

    public void MusicChange()
    {
        PlayerPrefs.SetFloat(musicName, music.value);
        UpdateValues();
    }

    public void SfxChange()
    {
        PlayerPrefs.SetFloat(sfxName, sfx.value);
        UpdateValues();
    }

    public void AmbientChange()
    {
        PlayerPrefs.SetFloat(ambientName, ambient.value);
        UpdateValues();
    }

    public void ResolutionChange()
    {
        PlayerPrefs.SetString(resolutionName, resOptions[resolution.value]);
        UpdateValues();
    }

    public void ColorBlindChange()
    {
        PlayerPrefs.SetInt(colorblindName, colorblind.isOn ? 1 : 0);
        UpdateValues();
    }

    public void FontTypeChange()
    {
        PlayerPrefs.SetString(fontTypeName, fontOptions[fontType.value]);
        UpdateValues();
    }

    public void Return()
    {
        UIController.Instance.CloseOverlays();
    }
}

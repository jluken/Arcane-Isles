using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavesMenuScript : MenuScreen
{
    public GameObject savesMenu;

    public static SavesMenuScript Instance;

    private bool active;

    private bool saveMode;

    public GameObject SaveListDisplay;
    private List<GameObject> savePanels;
    public GameObject SavePanelPrefab;
    private string activeSave;
    public TMP_Text currentSaveName;
    public Button saveGameButton;
    public Button deleteGameButton;
    public Button loadGameButton;
    public Button newSaveGameButton;

    public GameObject OverwritePanel;
    public GameObject DeletePanel;
    public GameObject NewSavePanel;

    public TMP_InputField saveName;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        savePanels = new List<GameObject>();
        activeSave = null;
    }

    public void SetSaveMode()
    {
        saveMode = true;
    }

    public void SetLoadMode()
    {
        saveMode = false;
    }

    public override void ActivateMenu()
    {
        PopulateSaves();
        savesMenu.SetActive(true);
        newSaveGameButton.gameObject.SetActive(saveMode);
        saveGameButton.gameObject.SetActive(saveMode);
        loadGameButton.gameObject.SetActive(!saveMode);
        active = true;
    }

    public override void DeactivateMenu()
    {
        savesMenu.SetActive(false);
        active = false;
    }

    public override bool IsActive()
    {
        return active;
    }

    public void Return()
    {
        UIController.Instance.CloseOverlays();
    }

    private void PopulateSaves()
    {
        activeSave = null;
        currentSaveName.text = "";
        saveGameButton.interactable = false;
        deleteGameButton.interactable = false;
        loadGameButton.interactable = false;

        foreach (GameObject panel in savePanels)
        {
            Destroy(panel);
        }
        savePanels.Clear();
        var saveNames = SaveSystem.GetSaves();
        var saveVals = new List<Dictionary<string, string>>();
        foreach (string name in saveNames)
        {
            var saveVal = new Dictionary<string, string>();
            saveVal.Add("name", name);
            saveVal.Add("time", SaveSystem.LoadGame(name).saveTime);
            saveVals.Add(saveVal);
        }
        saveVals = saveVals.OrderByDescending(x => DateTime.Parse(x["time"])).ToList();
        foreach (var saveVal in saveVals)
        {
            savePanels.Add(Instantiate(SavePanelPrefab, SaveListDisplay.transform));
            savePanels.LastOrDefault().GetComponent<SavePanel>().saveName = saveVal["name"];
            savePanels.LastOrDefault().GetComponent<SavePanel>().saveNameText.text = saveVal["name"];
            savePanels.LastOrDefault().GetComponent<SavePanel>().saveDateText.text = saveVal["time"];
            savePanels.LastOrDefault().GetComponent<Button>().onClick.AddListener(() => SetActiveSave(saveVal["name"]));        
        }
        ClosePanel();
    }

    private void SetActiveSave(string saveName)
    {
        activeSave = saveName;
        currentSaveName.text = saveName;
        saveGameButton.interactable = true;
        deleteGameButton.interactable = true;
        loadGameButton.interactable = true;
    }

    public void OpenNewSave()
    {
        saveName.placeholder.GetComponent<TextMeshProUGUI>().text = SaveSystem.GetNextSave();
        NewSavePanel.SetActive(true);
    }

    public void NewSave()
    {
        SaveSystem.SaveGame(saveName.text);
        PopulateSaves();
    }

    public void OpenOverwrite()
    {
        OverwritePanel.SetActive(true);
    }

    public void SetSave()
    {
        SaveSystem.SaveGame(activeSave);
        PopulateSaves();
    }

    public void LoadGame()
    {
        StartCoroutine(SceneLoader.Instance.LoadFromData(SaveSystem.LoadGame(activeSave)));
    }

    public void OpenDelete()
    {
        DeletePanel.SetActive(true);
    }

    public void DeleteSave()
    {
        SaveSystem.DeleteSave(activeSave);
        PopulateSaves();
    }

    public void ClosePanel()
    {
        NewSavePanel.SetActive(false);
        OverwritePanel.SetActive(false);
        DeletePanel.SetActive(false);
    }
}

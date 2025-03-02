using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
//ing UnityEngine.UIElements;

public class UIScript : MonoBehaviour  // TODO: set all UI canvases as subcanvas to one master canvas that can switch between them 
{
    public GameObject charPortrait;
    public Image healthBarBackground;
    public Image healthBar;
    public TMP_Text Health;
    public GameObject ButtonMenu;
    public GameObject TextMenu;
    public bool UIActive;  // TODO: better handled by UI manager

    public GameObject player; // TODO: these assignments will be handled with a controller once swapping players is functional
    private CharStats charStats;
    public GameObject cam;
    private camScript camScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start UI");
        charStats = player.GetComponent<CharStats>();
        charStats.updateStatEvent += UpdateStats;
        ActivateUI();
        camScript = cam.GetComponent<camScript>();
    }

    public void UpdateStats()
    {
        Debug.Log("Update stat UI");
        charPortrait.GetComponent<Image>().sprite = charStats.charImage;
        var maxHealth = charStats.currMaxHealth();
        var currHealth = charStats.currHealth();
        var fullWidth = healthBarBackground.rectTransform.sizeDelta.x;
        var newWidth = fullWidth * ((float)currHealth / maxHealth);

        healthBar.rectTransform.sizeDelta = new Vector2(newWidth, healthBar.rectTransform.sizeDelta.y);
        healthBar.transform.localPosition = new Vector3(((fullWidth-newWidth) / -2.0f), 0, 0);
        Health.text = currHealth + "/" + maxHealth;
    }

    public void DeactivateUI()
    {
        charPortrait.SetActive(false);
        ButtonMenu.SetActive(false);
        TextMenu.SetActive(false);
        UIActive = false;
        cam.GetComponent<camScript>().CamHalt(true); // TODO: for now this will only allow camera to move when "normal" UI is open - handled by UI manager later
    }

    public void ActivateUI()
    {
        charPortrait.SetActive(true);
        ButtonMenu.SetActive(true);
        TextMenu.SetActive(true);
        UIActive = true;
        cam.GetComponent<camScript>().CamHalt(false);
    }
}

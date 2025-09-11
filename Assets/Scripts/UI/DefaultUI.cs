using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
//ing UnityEngine.UIElements;

public class DefaultUI : MenuScreen 
{
    public GameObject charPortrait;
    public Image healthBarBackground;
    public Image healthBar;
    public TMP_Text Health;
    public GameObject ButtonMenu;
    //public GameObject TextMenu;
    public bool UIActive;

    public GameObject player;
    private CharStats charStats;
    public GameObject cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start UI");
        charStats = player.GetComponent<CharStats>();
        charStats.updateStatEvent += UpdateStats;
    }

    public void UpdateStats()
    {
        Debug.Log("Update stat UI");
        charPortrait.GetComponent<Image>().sprite = charStats.charImage;
        var maxHealth = charStats.GetCurrStat(CharStats.StatVal.maxHealth);
        var currHealth = charStats.GetCurrStat(CharStats.StatVal.health);
        var fullWidth = healthBarBackground.rectTransform.sizeDelta.x;
        var newWidth = fullWidth * ((float)currHealth / maxHealth);

        healthBar.rectTransform.sizeDelta = new Vector2(newWidth, healthBar.rectTransform.sizeDelta.y);
        healthBar.transform.localPosition = new Vector3(((fullWidth-newWidth) / -2.0f), 0, 0);
        Health.text = currHealth + "/" + maxHealth;
    }

    public override void DeactivateMenu()
    {
        charPortrait.SetActive(false);
        ButtonMenu.SetActive(false);
        //TextMenu.SetActive(false);
        UIActive = false;
    }

    public override void ActivateMenu()
    {
        charPortrait.SetActive(true);
        ButtonMenu.SetActive(true);
        //TextMenu.SetActive(true);
        UIActive = true;
    }

    public override bool IsActive()
    {
        return UIActive;
    }

    public override bool overlay => false;
}

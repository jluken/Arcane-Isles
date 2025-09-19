using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
//ing UnityEngine.UIElements;

public class DefaultUI : MenuScreen 
{
    public static DefaultUI Instance { get; private set; }

    [Serializable]
    public class CharIconData
    {
        public int charSlot;
        public GameObject charPortrait;
        public Image healthBarBackground;
        public Image healthBar;
        public TMP_Text Health;
    }

    public CharIconData[] charIcons = new CharIconData[4];

    public GameObject ButtonMenu;
    //public GameObject TextMenu;
    public bool UIActive;

    //private CharStats charStats;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start UI");
        //charStats = player.GetComponent<CharStats>();
        //charStats.updateStatEvent += UpdateStats;
    }

    public void UpdateStats()
    {
        Debug.Log("Update stat UI");
        var currentParty = PartyController.Instance.party;

        foreach (var charIcon in charIcons) { 
            if (charIcon.charSlot > currentParty.Count - 1) charIcon.charPortrait.SetActive(false);
            else
            {
                charIcon.charPortrait.SetActive(true);
                var partyMemberStats = currentParty[charIcon.charSlot].GetComponent<CharStats>();
                charIcon.charPortrait.GetComponent<Image>().sprite = partyMemberStats.charImage;
                var maxHealth = partyMemberStats.GetCurrStat(CharStats.StatVal.maxHealth);
                var currHealth = partyMemberStats.GetCurrStat(CharStats.StatVal.health);
                var fullWidth = charIcon.healthBarBackground.rectTransform.sizeDelta.x;
                var newWidth = fullWidth * ((float)currHealth / maxHealth);

                charIcon.healthBar.rectTransform.sizeDelta = new Vector2(newWidth, charIcon.healthBar.rectTransform.sizeDelta.y);
                charIcon.healthBar.transform.localPosition = new Vector3(((fullWidth - newWidth) / -2.0f), 0, 0);
                charIcon.Health.text = currHealth + "/" + maxHealth;

                if (currentParty[charIcon.charSlot].gameObject == PartyController.Instance.leaderObject)
                {
                    // TODO: For UI stage: highlight selected party member
                }
            }

        }
    }

    public override void DeactivateMenu()
    {
        foreach (var charIcon in charIcons) { charIcon.charPortrait.SetActive(false); }
        ButtonMenu.SetActive(false);
        //TextMenu.SetActive(false);
        UIActive = false;
    }

    public override void ActivateMenu()
    {
        UpdateStats();
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

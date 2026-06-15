using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class charIcon : MonoBehaviour
{
    public Image charPic;
    public Image border;

    public Image healthBarBackground;
    public Image healthBar;
    public TMP_Text Health;

    public GameObject statusBar;
    public Sprite stopSprite;

    private Character iconChar;
    private bool partyIcon;

    private List<string> statuses;
    private List<Image> statusIcons;

    void Awake()
    {
        statuses = new();
        statusIcons = new();
    }

    public void UpdateIcon(Character npcChar, bool isPartyIcon = false)
    {
        iconChar = npcChar;
        partyIcon = isPartyIcon;
        var partyMemberStats = npcChar.charStats;
        //Debug.Log("Current party icon member: " + npcChar);
        charPic.sprite = partyMemberStats.charImage;
        var maxHealth = partyMemberStats.GetCurrStat(CharStats.StatVal.maxHealth);
        var currHealth = partyMemberStats.GetCurrStat(CharStats.StatVal.health);
        var fullWidth = healthBarBackground.rectTransform.sizeDelta.x;
        var newWidth = fullWidth * ((float)currHealth / maxHealth);

        healthBar.rectTransform.sizeDelta = new Vector2(newWidth, healthBar.rectTransform.sizeDelta.y);
        healthBar.transform.localPosition = new Vector3(((fullWidth - newWidth) / -2.0f), 0, 0);
        Health.text = currHealth + "/" + maxHealth;

        border.gameObject.SetActive(npcChar.IsActive);

        // TODO: (add eventually status effects and refactor)
        var oldStatuses = statuses.ToList();
        if (npcChar.GetComponent<PartyMember>() != null && !npcChar.GetComponent<PartyMember>().CanFollow())
        {   
            if (!statuses.Contains("stay")) statuses.Add("stay");    
        }
        else
        {
            statuses.Remove("stay");
        }
        if (!statuses.SequenceEqual(oldStatuses))
        {
            foreach (var statusIcon in statusIcons) Destroy(statusIcon.gameObject);
            statusIcons.Clear();
            foreach (var status in statuses)
            {
                GameObject icon = new("StatusIcon");
                icon.transform.SetParent(statusBar.transform, false);
                icon.AddComponent<CanvasRenderer>();
                icon.AddComponent<Image>();
                icon.GetComponent<Image>().sprite = stopSprite;
                statusIcons.Add(icon.GetComponent<Image>());
            }
        }
    }

    public void SelectChar()
    {
        if (partyIcon) PartyController.Instance.SelectChar(iconChar.GetComponent<PartyMember>());
        else camScript.Instance.TrackObj(iconChar.gameObject);
    }
}

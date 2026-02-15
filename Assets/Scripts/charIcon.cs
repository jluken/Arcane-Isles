using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class charIcon : MonoBehaviour
{

    public Image healthBarBackground;
    public Image healthBar;
    public TMP_Text Health;

    private NPC iconChar;
    private bool partyIcon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void UpdateIcon(NPC npcChar, bool isPartyIcon = false)
    {
        iconChar = npcChar;
        partyIcon = isPartyIcon;
        var partyMemberStats = npcChar.charStats;
        //Debug.Log("Current party icon member: " + npcChar);
        GetComponent<Image>().sprite = partyMemberStats.charImage;
        var maxHealth = partyMemberStats.GetCurrStat(CharStats.StatVal.maxHealth);
        var currHealth = partyMemberStats.GetCurrStat(CharStats.StatVal.health);
        var fullWidth = healthBarBackground.rectTransform.sizeDelta.x;
        var newWidth = fullWidth * ((float)currHealth / maxHealth);

        healthBar.rectTransform.sizeDelta = new Vector2(newWidth, healthBar.rectTransform.sizeDelta.y);
        healthBar.transform.localPosition = new Vector3(((fullWidth - newWidth) / -2.0f), 0, 0);
        Health.text = currHealth + "/" + maxHealth;

        if (npcChar.IsActive)
        {
            // TODO: For UI stage: highlight selected party member(s)
        }
    }

    public void SelectChar()
    {
        if (partyIcon) PartyController.Instance.SelectChar(iconChar.GetComponent<PartyMember>());
        else camScript.Instance.TrackObj(iconChar.gameObject);
    }
}

using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InfoBox : MenuScreen
{
    public GameObject infoBox;
    public TMP_Text infoText;
    // TODO: maybe add option for icon in horizontal layout?

    private bool infoActive;

    public static InfoBox Instance { get; private set; }


    void Awake()
    {
        Instance = this;
    }

    public override void DeactivateMenu()
    {
        infoBox.SetActive(false);
        infoActive = false;
    }

    public void SetInfo(Vector3 pos, string text)
    {
        infoText.text = text;

        var menuDims = infoBox.GetComponent<RectTransform>().rect;
        var height = menuDims.height;
        var width = menuDims.width;
        infoBox.transform.position = new Vector3(pos.x + width / 2, pos.y - height / 2, 0);
    }

    public override void ActivateMenu()
    {
        infoBox.SetActive(true);
        infoActive = true;
    }

    public override bool IsActive()
    {
        return infoActive;
    }
}

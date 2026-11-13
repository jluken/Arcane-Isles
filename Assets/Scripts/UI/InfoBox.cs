using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InfoBox : MenuScreen
{
    public GameObject infoBox;
    public TMP_Text titleText;
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

    public void SetInfo(Vector3 pos, string title, string text)
    {
        titleText.text = title;
        infoText.text = text;

        var menuDims = infoBox.GetComponent<RectTransform>().rect;
        var height = menuDims.height;
        var width = menuDims.width;
        infoBox.transform.position = BoxPos(pos, menuDims);
    }

    private Vector3 BoxPos(Vector3 point, Rect menuDims)
    {
        // Ensure placing away from screen edge
        var xPos = (point.x >= Screen.width / 2) ? point.x - menuDims.width / 2 : point.x + menuDims.width / 2;
        var yPos = (point.y >= Screen.height / 2) ? point.y - 1 : point.y + 1 + ( menuDims.height);// (int)(point.y + menuDims.height);
        return new Vector3(xPos, yPos);
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

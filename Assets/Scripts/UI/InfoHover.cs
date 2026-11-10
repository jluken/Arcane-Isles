using UnityEngine;
using UnityEngine.EventSystems;

// Use only on UI elements
public class InfoHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string title;
    public string text;

    private bool isMouseOver;

    // Update is called once per frame
    void Update()
    {
        if (isMouseOver)
        {
            UIController.Instance.ActivateInfobox(title, text);
        }
    }

    public bool IsMouseOver { get; private set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    // Triggers exactly when the cursor leaves the UI element's bounds
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        UIController.Instance.DeactivateInfobox();
    }
}

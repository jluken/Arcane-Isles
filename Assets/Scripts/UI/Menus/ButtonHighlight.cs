using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text buttonText;

    private bool mouse_over = false;

    void Update()
    {
        if (mouse_over) buttonText.fontStyle |= FontStyles.Underline;
        else buttonText.fontStyle &= ~FontStyles.Underline;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
    }
}

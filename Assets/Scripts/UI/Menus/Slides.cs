using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Slides : MenuScreen
{
    public GameObject slide;
    public bool slideOpen;

    private SlideshowImages slideshow;
    private int slideNum;

    private InputActionMap uiActions;

    private void Awake()
    {
        uiActions = InputSystem.actions.FindActionMap("UI");
    }

    private void Start()
    {
        uiActions.FindAction("Click").performed += (sender) => NextSlide();
        uiActions.FindAction("Bump").performed += (sender) => NextSlide();
        uiActions.FindAction("Submit").performed += (sender) => NextSlide();
    }

    public override void DeactivateMenu()
    {
        slide.SetActive(false);
        slideOpen = false;
    }

    public override void ActivateMenu()
    {
        slide.SetActive(true);
        slideOpen = true;
        slideNum = -1;
        NextSlide();
    }

    public void SetSlides(SlideshowImages images)
    {
        slideshow = images;
    }

    public void NextSlide()
    {
        if (!slideOpen) return;
        slideNum++;
        if(slideNum >= slideshow.slides.Count) UIController.Instance.ActivateDefaultScreen();
        else SetSlide(slideshow.slides[slideNum]);
    }

    public void SetSlide(Sprite sprite)
    {
        slide.GetComponent<Image>().sprite = sprite;
    }

    public override bool IsActive()
    {
        return slideOpen;
    }
}

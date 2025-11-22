using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillCheckManager : MonoBehaviour
{
    public static SkillCheckManager Instance { get; private set; }
    public GameObject skillMenu;
    public TMP_Text checkDesc;
    public TMP_Text modifierText;

    enum RollState
    {
        NotRolled,
        Win,
        Fail
    }

    public Sprite[] diceFaces;
    public Image d1Face;
    public Image d2Face;

    private RollState rollState;

    public delegate void FailEvent();
    public static event FailEvent failEvent;
    public delegate void SuccessEvent();
    public static event SuccessEvent successEvent;

    //private SelectionController selectionController;

    private float[] rollProbs = { 100, 100, 100, 97.2f, 91.6f, 83.3f, 72.2f, 58.3f, 41.7f, 27.8f, 16.7f, 8.3f, 2.8f };  // TODO: possibly replace with dynamic sum formula
    private int moddedDC;

    private void Awake()
    {
        Instance = this;
        failEvent = null;
        successEvent = null;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skillMenu.SetActive(false);
        rollState = RollState.NotRolled;
        //selectionController = SelectionController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ActivateSkillCheck(CharStats.StatVal stat, int dc, (string, int)[] modifiers)  // TODO: reevaluate action approach after dialog is implemented
    {
        // TODO: the dialog will be a list of scriptable objects, each containing a dialog chunk, a face, a color (face and color default to what was before if not specified),
        // as well as a toggle for whether to take input. A similar toggle will also exist for a "skill check" input, which will branch it like normal selection input (Skill check should just return true/false)
        // Find a wau to make it so that, as a dialog chunk option, it can call a specified script (maybe script object? Unity events and subscribe?)
        Debug.Log("Activate check");
        Time.timeScale = 0; // Pauses time
        moddedDC = dc - modifiers.Select(x => x.Item2).Sum();
        var percent = 100f;
        if (0 <= moddedDC && moddedDC < 13) { percent = rollProbs[moddedDC]; }
        else if (moddedDC >= 13) { percent = 0f; }
        checkDesc.text = stat + " " + dc + " (" + percent + "%)";
        var modText = "";
        foreach ((string, int) mod in modifiers)
        {
            if (mod.Item2 > 0) { modText += "<color=green>" + mod.Item1 + " +" + mod.Item2 + "</color>\n"; }
            else if (mod.Item2 < 0) { modText += "<color=red>" + mod.Item1 + " -" + Math.Abs(mod.Item2) + "</color>\n"; }
        }
        modifierText.text = modText;
        skillMenu.SetActive(true);
        rollState = RollState.NotRolled;
    }

    public void Roll()
    {
        System.Random rnd = new System.Random();
        int d1 = rnd.Next(1, 7);
        int d2 = rnd.Next(1, 7);

        d1Face.sprite = diceFaces[d1 - 1];
        d2Face.sprite = diceFaces[d2 - 1];

        if(d1 + d2 >= moddedDC)
        {
            rollState = RollState.Win;
        }
        else
        {
            rollState = RollState.Fail;
        }
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && rollState != RollState.NotRolled)
        {
            // Exit out and trigger rolled events
            skillMenu.SetActive(false);
            if (rollState == RollState.Win) { successEvent.Invoke(); } 
            else { failEvent.Invoke(); }
            rollState = RollState.NotRolled;
            // Clear listeners to roll event
            failEvent = null;
            successEvent = null;
            SelectionController.Instance.Deselect();
            Time.timeScale = 1;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class rollButton : MonoBehaviour
{
    public SkillCheckManager skillManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skillManager = GameObject.Find("SkillCheckCanvas").GetComponent<SkillCheckManager>();
    }

    public void RollClick()
    {
        skillManager.Roll();
    }
}

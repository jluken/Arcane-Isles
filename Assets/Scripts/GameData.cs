using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    public float gameTime;

    void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CombatManager.Instance.combatActive) gameTime += Time.deltaTime;  // Time incremented by CombatManager during combat
    }
}

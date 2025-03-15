using System;
using System.Data;
using UnityEngine;

public class JournalScript : MonoBehaviour
{
    public GameObject journal;

    private bool journalOpen;

    void Start()
    {
        DeactivateJournal();
    }

    void Update()
    {
        if (!journalOpen && Input.GetKeyDown(KeyCode.J))
        {
            ActivateJournal();
        }
        else if (journalOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.J)))
        {
            DeactivateJournal();
        }
    }

    public void DeactivateJournal()
    {
        journal.SetActive(false);
        journalOpen = false;
    }

    public void ActivateJournal()
    {
        journal.SetActive(true);
        journalOpen = true;
    }
}

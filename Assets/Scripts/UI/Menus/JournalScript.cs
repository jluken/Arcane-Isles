using System;
using System.Data;
using UnityEngine;

public class JournalScript : MenuScreen
{
    public GameObject journal;

    private bool journalOpen;

    void Start()
    {
        //DeactivateJournal();
    }

    void Update()
    {
        //if (!journalOpen && Input.GetKeyDown(KeyCode.J))
        //{
        //    ActivateJournal();
        //}
        //else if (journalOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.J)))
        //{
        //    DeactivateJournal();
        //}
    }

    public override void DeactivateMenu()
    {
        journal.SetActive(false);
        journalOpen = false;
    }

    public override void ActivateMenu()
    {
        journal.SetActive(true);
        journalOpen = true;
    }

    public override bool IsActive()
    {
        return journalOpen;
    }

    public override bool overlay => true;
}

using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharStats))]
public class PlayerChar : PartyMember
{
    public static PlayerChar Instance;

    public override bool mainChar { get; } = true;

    public void Awake()
    {
        Instance = this;
    }

}

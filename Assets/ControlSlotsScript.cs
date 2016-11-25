﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class ControlSlotsScript : NetworkBehaviour
{
    /// <summary>
    /// The representation of what slots are available for players and how they are populated.
    /// </summary>
    public ControlSlot[] slots = new ControlSlot[4];

    void Awake()
    {
        // Default control slots layout
        slots[0] = new ControlSlot() { controlType = ControlType.Player };
        slots[1] = new ControlSlot() { controlType = ControlType.AI };
        slots[2] = new ControlSlot() { controlType = ControlType.Closed };
        slots[3] = new ControlSlot() { controlType = ControlType.Closed };

        DontDestroyOnLoad(gameObject);

        // Solution to the madness of duplicate objects somehow being retained (WTF?)
        // See: http://answers.unity3d.com/answers/485933/view.html
        if (FindObjectsOfType(GetType()).Length > 1) Destroy(gameObject);
    }
}

/// <summary>
/// Who controls the character in a given slot.
/// </summary>
public enum ControlType
{
    /// <summary>
    /// There is no player in the slot.
    /// </summary>
    Closed,

    /// <summary>
    /// The human host of the game.
    /// </summary>
    Player,

    /// <summary>
    /// An AI controlled player.
    /// </summary>
    AI
}

/// <summary>
/// A player slot.
/// </summary>
[Serializable]
public class ControlSlot
{
    /// <summary>
    /// Which type of control this player is.
    /// </summary>
    public ControlType controlType;

    /// <summary>
    /// The name of the character that this slot has selected.
    /// </summary>
    public string chosenCharacter;

    public override string ToString()
    {
        return "{controlType: " + controlType + ", chosenCharacter: " + chosenCharacter + "}";
    }
}
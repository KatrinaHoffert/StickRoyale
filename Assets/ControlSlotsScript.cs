using UnityEngine;
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
        slots[0] = new ControlSlot() { controlType = ControlType.Host };
        slots[1] = new ControlSlot() { controlType = ControlType.Network };
        slots[2] = new ControlSlot() { controlType = ControlType.Closed };
        slots[3] = new ControlSlot() { controlType = ControlType.Closed };

        DontDestroyOnLoad(gameObject);
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
    Host,

    /// <summary>
    /// A network slot for other human clients.
    /// </summary>
    Network,

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

    /// <summary>
    /// The ID of the populated network player, if this control slot is a network one. Null symbolizes
    /// an empty slot.
    /// </summary>
    public string networkPlayerId;

    public override string ToString()
    {
        return "{controlType: " + controlType + ", chosenCharacter: " + chosenCharacter + ", networkPlayerId: " + networkPlayerId + "}";
    }
}
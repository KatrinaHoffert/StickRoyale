﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// A part of the player that is used for communicating menu choices.
/// </summary>
public class PlayerMenuCommunications : NetworkBehaviour
{
    [Command]
    public void CmdChooseCharacter(int slot, string character)
    {
        ChooseCharacter(slot, character);
        RpcChooseCharacter(slot, character);
    }

    [ClientRpc]
    public void RpcChooseCharacter(int slot, string character)
    {
        ChooseCharacter(slot, character);
    }

    /// <summary>
    /// Sets the character for this slot as the the chosen character, then updated the GUI.
    /// </summary>
    /// <param name="slot">The slot of the player.</param>
    /// <param name="character">The character they chose.</param>
    private void ChooseCharacter(int slot, string character)
    {
        Debug.Log("Player " + slot + " has chosen character " + character);
        GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>().slots[slot].chosenCharacter = character;
        GameObject.Find("Canvas").GetComponent<CharacterSelectMenuScript>().UpdateCharacterImages();
    }
}
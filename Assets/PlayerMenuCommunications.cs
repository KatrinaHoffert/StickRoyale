using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

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
    
    /// <summary>
    /// Called by the host to send clients its control slots (serialized into JSON).
    /// </summary>
    public void SendHostSlots()
    {
        var slots = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>().slots;

        // By sheer madness, JsonUtility cannot serialize arrays, so gotta serialize the elements individually
        RpcSendHostSlots(JsonUtility.ToJson(slots[0]), JsonUtility.ToJson(slots[1]), JsonUtility.ToJson(slots[2]), JsonUtility.ToJson(slots[3]));
    }

    /// <summary>
    /// Called on the clients to update the control slots to the hosts's data and the GUI for that accordingly.
    /// The parameters are JSON objects for each control slot (since JsonUtility cannot deserialize
    /// arrays).
    /// </summary>
    [ClientRpc]
    public void RpcSendHostSlots(string controlSlot0, string controlSlot1, string controlSlot2, string controlSlot3)
    {
        // Only need to execute this on the clients
        if (!isServer)
        {
            var slots = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>().slots;
            JsonUtility.FromJsonOverwrite(controlSlot0, slots[0]);
            JsonUtility.FromJsonOverwrite(controlSlot1, slots[1]);
            JsonUtility.FromJsonOverwrite(controlSlot2, slots[2]);
            JsonUtility.FromJsonOverwrite(controlSlot3, slots[3]);

            // Could JsonUtility be any freaking buggier than it is? Nulls got converted to empty strings. Fix that.
            foreach(var slot in slots)
            {
                if (slot.chosenCharacter == "") slot.chosenCharacter = null;
                if (slot.networkPlayerId == "") slot.networkPlayerId = null;
            }

            // Update dropdowns and images
            var charMenu = GameObject.Find("Canvas").GetComponent<CharacterSelectMenuScript>();
            charMenu.UpdateControlDropdowns();
            charMenu.UpdateCharacterImages();
        }
    }
}

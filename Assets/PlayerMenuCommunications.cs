using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

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
        RpcSendHostSlots(JsonConvert.SerializeObject(slots));
    }

    /// <summary>
    /// Called on the clients to update the control slots to the hosts's data and the GUI for that accordingly.
    /// The parameters are JSON objects for each control slot (since JsonUtility cannot deserialize
    /// arrays).
    /// </summary>
    [ClientRpc]
    public void RpcSendHostSlots(string jsonControlSlots)
    {
        // Only need to execute this on the clients
        if (!isServer)
        {
            var slots = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>().slots;
            var deserializedControlSlots = JsonConvert.DeserializeObject<ControlSlot[]>(jsonControlSlots);
            for(int slot = 0; slot < slots.Length; ++slot)
            {
                slots[slot] = deserializedControlSlots[slot];
            }

            // Update dropdowns and images
            var charMenu = GameObject.Find("Canvas").GetComponent<CharacterSelectMenuScript>();
            charMenu.UpdateControlDropdowns();
            charMenu.UpdateCharacterImages();
        }
    }
}

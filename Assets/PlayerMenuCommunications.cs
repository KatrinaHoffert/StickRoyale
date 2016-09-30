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
    /// <summary>
    /// Called by the client to inform the host of having chosen a character. Host will then
    /// inform everyone else (including itself).
    /// 
    /// Perhaps should consider taking in the unique ID instead of the slot in case that happens to
    /// change at the very same time? Seems like a minor issue, though, and doesn't easily occur.
    /// </summary>
    /// <param name="slot">Slot of the player.</param>
    /// <param name="character">Character they chose.</param>
    [Command]
    public void CmdChooseCharacter(int slot, string character)
    {
        RpcChooseCharacter(slot, character);
    }

    /// <summary>
    /// Called by the host to inform all clients (including itself) that a certain slot was assigned a
    /// certain character. We update the control slots and the GUI accordingly.
    /// </summary>
    /// <param name="slot">Slot of the player.</param>
    /// <param name="character">Character they chose.</param>
    [ClientRpc]
    public void RpcChooseCharacter(int slot, string character)
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
    /// </summary>
    /// <param name="jsonControlSlots">A JSON representation of the <see cref="ControlSlotsScript.slots"/>.</param>
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

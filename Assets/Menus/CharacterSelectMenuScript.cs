﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class CharacterSelectMenuScript : MonoBehaviour
{
    /// <summary>
    /// What scene the back button goes to (since we can arrive at this scene through either
    /// the new game or join game menus). Also used since which screen we came from determines
    /// whether or not we're a client.
    /// </summary>
    public static string backButtonTarget = "MainMenu";

    /// <summary>
    /// A reference to the control slots belonging to the `ControlSlots` object's `ControlSlotsScript`
    /// component. Here as a field because it's used consistently throughout this class.
    /// </summary>
    private ControlSlot[] controlSlots;

    void Start()
    {
        // Initialize the control slots
        controlSlots = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>().slots;

        // If we came from the main menu, we must be the host. Otherwise we're a client. This is the
        // easiest way to check this since the host might not be setup yet and hence we don't know if
        // we're a host or client (and the player objects might not have been spawned).
        if (backButtonTarget == "MainMenu")
        {
            // We're the host. This callback is actually called for every player join, but that's fine.
            GameObject.Find("NetworkManager").GetComponent<NetworkManagerScript>().ConnectHost(() =>
            {
                // Set the host's player ID
                controlSlots[0].networkPlayerId = Global.GetOurPlayer().uniquePlayerId;

                AssignUnassignedPlayersToNetworkSlots();
            });
        }
        else
        {
            // Non-hosts need to disable control dropdowns
            GameObject.Find("P1Control").GetComponent<Dropdown>().enabled = false;
            GameObject.Find("P2Control").GetComponent<Dropdown>().enabled = false;
            GameObject.Find("P3Control").GetComponent<Dropdown>().enabled = false;
        }

        // Host must be p0, so disable the select there (it's not a real dropdown anyway, since there's
        // only one option)
        GameObject.Find("P0Control").GetComponent<Dropdown>().enabled = false;
    }

    /// <summary>
    /// Called when 
    /// </summary>
    /// <param name="name"></param>
    public void CharacterButtonClicked(string name)
    {
        // Find our slot
        var ourPlayerId = Global.GetOurPlayer().uniquePlayerId;
        for(int slot = 0; slot < controlSlots.Length; ++slot)
        {
            if(controlSlots[slot].networkPlayerId == ourPlayerId)
            {
                Debug.Log("Selected character " + name + " for player " + slot);
                controlSlots[slot].chosenCharacter = name;
            }
        }
    }
    
    /// <summary>
    /// Called when the "back" button is pressed.
    /// </summary>
	public void BackButtonDown()
    {
        Debug.Log("Back");
        GameObject.Find("NetworkManager").GetComponent<NetworkManagerScript>().Disconnect();
        SceneManager.LoadScene(backButtonTarget);
    }

     public void ControlChange0(int value) { ControlChange(0, value); }
     public void ControlChange1(int value) { ControlChange(1, value); }
     public void ControlChange2(int value) { ControlChange(2, value); }
     public void ControlChange3(int value) { ControlChange(3, value); }

    /// <summary>
    /// Updates the control slots (<see cref="ControlSlotsScript.slots"/> with the set control changes as well
    /// as performs some checking. Particularly, there must always be a host.
    /// </summary>
    /// <param name="slot">The slot that changed.</param>
    /// <param name="value">The new value in the slot that changed.</param>
    private void ControlChange(int slot, int value)
    {
        // Map of the dropdown values to the actual control types
        Dictionary<int, ControlType> dropdownControlTypes = new Dictionary<int, ControlType>();
        dropdownControlTypes[0] = ControlType.Closed;
        dropdownControlTypes[1] = ControlType.Network;
        dropdownControlTypes[2] = ControlType.AI;

        Debug.Log("Set player in slot " + slot + " as control type " + dropdownControlTypes[value]);

        // Did we close a network slot? Move the network player to another available slot if there is one
        if (controlSlots[slot].controlType == ControlType.Network)
        {
            bool foundSlotForPlayer = false;
            for (int possibleSlot = 1; possibleSlot < controlSlots.Length; ++possibleSlot)
            {
                if (possibleSlot == slot) continue;
                // Found a slot -- switch the network player to this one
                if(controlSlots[possibleSlot].controlType == ControlType.Network && controlSlots[possibleSlot].networkPlayerId == null)
                {
                    Debug.Log("Network player in slot " + slot + " moved to slot " + possibleSlot);
                    controlSlots[possibleSlot].networkPlayerId = controlSlots[slot].networkPlayerId;
                    controlSlots[slot].networkPlayerId = null;
                    foundSlotForPlayer = true;
                    break;
                }
            }

            // If we didn't find a slot for them, kick 'em and clear the ID of the slot
            if(!foundSlotForPlayer)
            {
                Debug.Log("Network player in slot " + slot + " kicked because their slot was closed and no open slots could be found");
                var player = Global.GetPlayerById(controlSlots[slot].networkPlayerId);
                player.connectionToClient.Disconnect();
                controlSlots[slot].networkPlayerId = null;
            }
        }
        
        // And actually update the slot
        controlSlots[slot].controlType = dropdownControlTypes[value];

        UpdateCharacterImages();
    }

    /// <summary>
    /// Check if there's any new players that have not been assigned a slot. If so, assign them a
    /// slot. Otherwise, kick them.
    /// </summary>
    private void AssignUnassignedPlayersToNetworkSlots()
    {
        PlayerBase[] players = FindObjectsOfType<PlayerBase>();
        foreach (var player in players)
        {
            bool networkPlayerHasSlot = false;
            foreach (var slot in controlSlots)
            {
                if (slot.networkPlayerId == player.uniquePlayerId) networkPlayerHasSlot = true;
            }

            if (!networkPlayerHasSlot)
            {
                // Any free slots?
                bool foundFreeSlot = false;
                foreach (var slot in controlSlots)
                {
                    if (slot.controlType == ControlType.Network && slot.networkPlayerId == null)
                    {
                        slot.networkPlayerId = player.uniquePlayerId;
                        foundFreeSlot = true;
                        break;
                    }
                }

                if (!foundFreeSlot)
                {
                    Debug.Log("A player attempted to connect, but there was no network slots. They were disconnected.");
                    player.connectionToClient.Disconnect();
                }
            }
        }
    }

    /// <summary>
    /// Updates all the character images based on the data in the control slots (ie, <see cref="ControlSlotsScript.slots"/>).
    /// </summary>
    private void UpdateCharacterImages()
    {
        var allImages = new GameObject[]
        {
            GameObject.Find("P0Image"),
            GameObject.Find("P1Image"),
            GameObject.Find("P2Image"),
            GameObject.Find("P3Image")
        };

        for(int slot = 0; slot < allImages.Length; ++slot)
        {
            // Decide what the image should be for this slot
            string imageName;
            if (controlSlots[slot].controlType == ControlType.Closed) imageName = "CharacterImages/EmptySlot";
            else if (controlSlots[slot].chosenCharacter == null) imageName = "CharacterImages/UnknownCharacter";
            else imageName = "CharacterImages/" + controlSlots[slot].chosenCharacter;

            // Note that resources must be relative to the Resources folder.
            var loadedSprite = Resources.Load<Sprite>(imageName);
            if(loadedSprite != null)
            {
                var image = allImages[slot].GetComponent<Image>().sprite = loadedSprite;
            }
            else
            {
                Debug.LogError("Couldn't load image for " + imageName + " (slot " + slot + ")", this);
            }
        }
    }
}
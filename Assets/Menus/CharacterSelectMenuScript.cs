using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Networking.NetworkSystem;
using Newtonsoft.Json;

public class CharacterSelectMenuScript : MonoBehaviour
{
    /// <summary>
    /// What scene the back button goes to (since we can arrive at this scene through either
    /// the new game or join game menus). Also used since which screen we came from determines
    /// whether or not we're a client. Note that this is only properly set if we start the game
    /// in the main menu!
    /// </summary>
    public static string backButtonTarget = "MainMenu";


    public Sprite sprites;

    /// <summary>
    /// A reference to the control slots belonging to the <see cref="ControlSlotsScript.slots"/> 
    /// variable. Here as a field because it's used consistently throughout this class.
    /// </summary>
    private ControlSlot[] controlSlots;

    void Start()
    {
        // Initialize the control slots
        controlSlots = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>().slots;

        // All the "you" indicators should be disabled. They'll be selectively enabled depending on
        // which slot we're in.
        GameObject.Find("P0YouIndicator").GetComponent<Image>().enabled = false;
        GameObject.Find("P1YouIndicator").GetComponent<Image>().enabled = false;
        GameObject.Find("P2YouIndicator").GetComponent<Image>().enabled = false;
        GameObject.Find("P3YouIndicator").GetComponent<Image>().enabled = false;

        // If we came from the main menu, we must be the host. Otherwise we're a client. This is the
        // easiest way to check this since the host might not be setup yet and hence we don't know if
        // we're a host or client (and the player objects might not have been spawned).
        if (backButtonTarget == "MainMenu")
        {
            // We're the host. This callback is actually called for every player join, but that's fine.
            GameObject.Find("NetworkManager").GetComponent<NetworkManagerScript>().ConnectHost(() =>
            {
                // Set the host's player ID
                Global.GetOurPlayer().uniquePlayerId = Guid.NewGuid().ToString();
                controlSlots[0].networkPlayerId = Global.GetOurPlayer().uniquePlayerId;

                AssignUnassignedPlayersToNetworkSlots();
                UpdateCharacterImages();
            });
        }
        else
        {
            // Non-hosts need to disable control dropdowns and the start button
            GameObject.Find("P1Control").GetComponent<Dropdown>().enabled = false;
            GameObject.Find("P2Control").GetComponent<Dropdown>().enabled = false;
            GameObject.Find("P3Control").GetComponent<Dropdown>().enabled = false;
            GameObject.Find("StartGameButton").GetComponent<Button>().enabled = false;
        }

        // Host must be p0, so disable the select there (it's not a real dropdown anyway, since there's
        // only one option)
        GameObject.Find("P0Control").GetComponent<Dropdown>().enabled = false;
    }

    void FixedUpdate()
    {
        try
        {
            // Here we're having the host continuously check to see if any players disconnected, so that we can
            // cleanup their slot.
            if (!Global.PlayerIsClient()) ClearDisconnectedPlayers();
        }
        catch
        {
            // Do nothing, since it's normal for `PlayerIsClient` to fail if this ticks before the player prefabs are made
        }
    }

    /// <summary>
    /// Called when a character button is clicked.
    /// </summary>
    /// <param name="name">The name of the chosen character.</param>
    public void CharacterButtonClicked(string name)
    {
        // Find our slot
        var ourPlayerId = Global.GetOurPlayer().uniquePlayerId;
        for (int slot = 0; slot < controlSlots.Length; ++slot)
        {
            if (controlSlots[slot].networkPlayerId == ourPlayerId)
            {
                Debug.Log("Selected character " + name + " for player " + slot);
                controlSlots[slot].chosenCharacter = name;
                var playerComm = Global.GetOurPlayer().gameObject.GetComponent<PlayerMenuCommunications>();
                if (Global.PlayerIsClient()) playerComm.CmdChooseCharacter(slot, name);
                else playerComm.RpcChooseCharacter(slot, name);
            }
        }
        UpdateCharacterImages();
    }

    /// <summary>
    /// Called when the "back" button is pressed.
    /// </summary>
    public void BackButtonDown()
    {
        Debug.Log("Back");
        GameObject.Find("NetworkManager").GetComponent<NetworkManagerScript>().Disconnect();

        // Cleanup stuff that otherwise get left behind
        DestroyImmediate(GameObject.Find("ControlSlots"));
        DestroyImmediate(GameObject.Find("NetworkManager"));

        SceneManager.LoadScene(backButtonTarget);
    }

    public void ControlChange0(int value) { ControlChange(0, value); }
    public void ControlChange1(int value) { ControlChange(1, value); }
    public void ControlChange2(int value) { ControlChange(2, value); }
    public void ControlChange3(int value) { ControlChange(3, value); }

    /// <summary>
    /// Map of the dropdown values to the actual control types.
    /// </summary>
    private Dictionary<int, ControlType> GetDropdownControlMapping()
    {
        Dictionary<int, ControlType> dropdownControlTypes = new Dictionary<int, ControlType>();
        dropdownControlTypes[0] = ControlType.Closed;
        dropdownControlTypes[1] = ControlType.Network;
        dropdownControlTypes[2] = ControlType.AI;
        return dropdownControlTypes;
    }

    /// <summary>
    /// Unity provides no way to change a dropdown without triggering its event handler. Which will cause
    /// bugs when we're trying to programmatically change the value. Hence, we must have a toggle that
    /// will disable the event handler for programmatic changes. ie, if this is true, the event handler
    /// does nothing.
    /// </summary>
    private bool programmaticControlChange = false;

    /// <summary>
    /// Updates the control slots (<see cref="ControlSlotsScript.slots"/> with the set control changes as well
    /// as performs some checking. Particularly, there must always be a host.
    /// </summary>
    /// <param name="slot">The slot that changed.</param>
    /// <param name="value">The new value in the slot that changed.</param>
    private void ControlChange(int slot, int value)
    {
        if (programmaticControlChange) return;

        Dictionary<int, ControlType> dropdownControlTypes = GetDropdownControlMapping();

        Debug.Log("Set player in slot " + slot + " as control type " + dropdownControlTypes[value]);

        // Did we close a network slot? Move the network player to another available slot if there is one
        if (controlSlots[slot].controlType == ControlType.Network && !String.IsNullOrEmpty(controlSlots[slot].networkPlayerId))
        {
            bool foundSlotForPlayer = false;
            for (int possibleSlot = 1; possibleSlot < controlSlots.Length; ++possibleSlot)
            {
                if (possibleSlot == slot) continue;
                // Found a slot -- switch the network player to this one
                if (controlSlots[possibleSlot].controlType == ControlType.Network && String.IsNullOrEmpty(controlSlots[possibleSlot].networkPlayerId))
                {
                    Debug.Log("Network player in slot " + slot + " moved to slot " + possibleSlot);
                    controlSlots[possibleSlot].networkPlayerId = controlSlots[slot].networkPlayerId;
                    controlSlots[slot].networkPlayerId = null;
                    foundSlotForPlayer = true;
                    break;
                }
            }

            // If we didn't find a slot for them, kick 'em and clear the ID of the slot
            if (!foundSlotForPlayer)
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
        
        Global.GetOurPlayer().gameObject.GetComponent<PlayerMenuCommunications>().SendHostSlots();
    }

    /// <summary>
    /// Called when the start game button is clicked. Will validate if the control slots are valid, and if so,
    /// transition to the next step.
    /// </summary>
    public void StartButtonClicked()
    {
        bool gameIsValid = true;
        string failureReason = "";

        for(int slot = 0; slot < controlSlots.Length; ++slot)
        {
            // The host and network slots must have characters selected and have an ID set
            if(controlSlots[slot].controlType == ControlType.Host || controlSlots[slot].controlType == ControlType.Network)
            {
                if (string.IsNullOrEmpty(controlSlots[slot].networkPlayerId))
                {
                    gameIsValid = false;
                    failureReason = "Player slot " + slot + " has not been populated by a network player";
                    break;
                }
                if (string.IsNullOrEmpty(controlSlots[slot].chosenCharacter))
                {
                    gameIsValid = false;
                    failureReason = "Player " + slot + " has not selected a character";
                    break;
                }
            }
            // No AI slots allowed right now
            else if(controlSlots[slot].controlType == ControlType.AI)
            {
                gameIsValid = false;
                failureReason = "Player " + slot + " is AI. AI has not been implemented yet!";
                break;
            }
        }

        if(gameIsValid)
        {
            // TODO: We're all good, load level select

            Debug.Log("Starting game. Chosen controls are: " + JsonConvert.SerializeObject(controlSlots));
            //NetworkManager.ServerChangeScene("level1");
            SceneManager.LoadScene("level1");
            
            PlayerBase[] players = FindObjectsOfType<PlayerBase>();
            foreach(var player in players)
            {
                
                player.gameObject.AddComponent<Rigidbody2D>();
                player.gameObject.AddComponent<SpriteRenderer>();
                player.gameObject.GetComponent<SpriteRenderer>().sprite = sprites;
                player.gameObject.AddComponent<BoxCollider2D>();
                player.gameObject.AddComponent<MovementScript>();
                player.gameObject.AddComponent<PlayerMovement>();
                
            }
            

        }
        else
        {
            Debug.Log("Cannot start game. Reason: " + failureReason);
        }
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
            // New player? Give them a unique ID and send that across the network.
            if(String.IsNullOrEmpty(player.uniquePlayerId))
            {
                player.uniquePlayerId = Guid.NewGuid().ToString();
                NetworkServer.SendToClientOfPlayer(player.gameObject, CustomMessageTypes.PlayerSetUniqueId, new StringMessage(player.uniquePlayerId));
                Debug.Log("New player detected. Given ID " + player.uniquePlayerId);
            }

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
                    if (slot.controlType == ControlType.Network && String.IsNullOrEmpty(slot.networkPlayerId))
                    {
                        slot.networkPlayerId = player.uniquePlayerId;
                        foundFreeSlot = true;

                        // Now that we've got a slot for the user, update all players' control slots to match ours
                        Global.GetOurPlayer().gameObject.GetComponent<PlayerMenuCommunications>().SendHostSlots();
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
    public void UpdateCharacterImages()
    {
        var allImages = new GameObject[]
        {
            GameObject.Find("P0Image"),
            GameObject.Find("P1Image"),
            GameObject.Find("P2Image"),
            GameObject.Find("P3Image")
        };

        for (int slot = 0; slot < allImages.Length; ++slot)
        {
            // Decide what the image should be for this slot
            string imageName;
            if (controlSlots[slot].controlType == ControlType.Closed) imageName = "CharacterImages/EmptySlot";
            else if (String.IsNullOrEmpty(controlSlots[slot].chosenCharacter)) imageName = "CharacterImages/UnknownCharacter";
            else imageName = "CharacterImages/" + controlSlots[slot].chosenCharacter;

            // Note that resources must be relative to the Resources folder.
            var loadedSprite = Resources.Load<Sprite>(imageName);
            if (loadedSprite != null)
            {
                allImages[slot].GetComponent<Image>().sprite = loadedSprite;
            }
            else
            {
                Debug.LogError("Couldn't load image for " + imageName + " (slot " + slot + ")", this);
            }
        }

        // Enable the "you" indicator for the slot we're in
        var ourId = Global.GetOurPlayer().uniquePlayerId;
        for(int slot = 0; slot < controlSlots.Length; ++slot)
        {
            if(controlSlots[slot].networkPlayerId == ourId)
            {
                GameObject.Find("P" + slot + "YouIndicator").GetComponent<Image>().enabled = true;
            }
            else
            {
                GameObject.Find("P" + slot + "YouIndicator").GetComponent<Image>().enabled = false;
            }
        }
    }

    /// <summary>
    /// Updates the control dropdowns based on the values of the control slots (which have presumably
    /// been updated from the host).
    /// </summary>
    public void UpdateControlDropdowns()
    {
        programmaticControlChange = true;

        // Flipping the mapping to map ControlTypes to the dropdown values
        Dictionary<ControlType, int> dropdownControlTypes = GetDropdownControlMapping().ToDictionary(item => item.Value, item => item.Key);

        for(int slot = 1; slot < controlSlots.Length; ++slot)
        {
            GameObject.Find("P" + slot + "Control").GetComponent<Dropdown>().value = dropdownControlTypes[controlSlots[slot].controlType];
        }

        programmaticControlChange = false;
    }

    /// <summary>
    /// Clears disconnected players from the control slots and alerts other clients of what changed.
    /// </summary>
    private void ClearDisconnectedPlayers()
    {
        bool madeChanges = false;
        PlayerBase[] players = FindObjectsOfType<PlayerBase>();
        for(int slot = 0; slot < controlSlots.Length; ++slot)
        {
            // If there's no players with the control slot's ID, clear the slot
            if(!String.IsNullOrEmpty(controlSlots[slot].networkPlayerId) && !players.Any(player => player.uniquePlayerId == controlSlots[slot].networkPlayerId))
            {
                Debug.Log("Slot " + slot + " is disconnected; clearing.");
                controlSlots[slot].chosenCharacter = null;
                controlSlots[slot].networkPlayerId = null;
                madeChanges = true;
            }
        }

        // If we made changes, let everyone else know
        if(madeChanges)
        {
            Global.GetOurPlayer().gameObject.GetComponent<PlayerMenuCommunications>().SendHostSlots();
        }
    }
}
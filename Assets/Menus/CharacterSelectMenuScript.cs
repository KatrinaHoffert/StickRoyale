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

/// <summary>
/// Scripting for the CharacterSelectMenu. This is the most complicated of the menus, as we have to
/// allow multiple players to select their characters and there's multiple control slots that each
/// player can have (see <see cref="ControlSlot"/>). The whole purpose of this menu is simply to
/// populate the <see cref="controlSlots"/>.
/// </summary>
public class CharacterSelectMenuScript : MonoBehaviour
{
    /// <summary>
    /// A reference to the control slots belonging to the <see cref="ControlSlotsScript.slots"/> 
    /// variable. Here as a field because it's used consistently throughout this class.
    /// </summary>
    public static ControlSlot[] controlSlots;

    private int selectingPlayer = 0;

    void Start()
    {
        // Initialize the control slots
        controlSlots = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>().slots;

        // Player must be p0, so disable the select there (it's not a real dropdown anyway, since there's
        // only one option)
        GameObject.Find("P0Control").GetComponent<Dropdown>().enabled = false;

        // We might have arrived here via back button on level select -- update the images in case.
        UpdateCharacterImages();

        // Disable the selecting images until they are needed
        GameObject.Find("P1Selecting").GetComponent<Image>().enabled = false;
        GameObject.Find("P2Selecting").GetComponent<Image>().enabled = false;
        GameObject.Find("P3Selecting").GetComponent<Image>().enabled = false;
    }

    /// <summary>
    /// Called when a character button is clicked.
    /// </summary>
    /// <param name="name">The name of the chosen character.</param>
    public void CharacterButtonClicked(string name)
    {
        Debug.Log("Selected character " + name + " for player " + selectingPlayer);
        controlSlots[selectingPlayer].chosenCharacter = name;
        UpdateCharacterImages();
    }
    
    public void ControlChange1(int value) { ControlChange(1, value); }
    public void ControlChange2(int value) { ControlChange(2, value); }
    public void ControlChange3(int value) { ControlChange(3, value); }
    
    /// <summary>
    /// Sets a player as the one we are choosing a character for
    /// </summary>
    /// <param name="playerNumber">Player's number (0-3).</param>
    public void PlayerSelected(int playerNumber)
    {
        if (controlSlots[playerNumber].controlType != ControlType.Closed)
        {
            selectingPlayer = playerNumber;

            // Enable the appropriate selecting image
            for(int i = 0; i < controlSlots.Length; ++i)
            {
                if(i == playerNumber)
                {
                    GameObject.Find("P" + i + "Selecting").GetComponent<Image>().enabled = true;
                }
                else
                {
                    GameObject.Find("P" + i + "Selecting").GetComponent<Image>().enabled = false;
                }
            }
        }
    }

    /// <summary>
    /// Map of the dropdown values to the actual control types.
    /// </summary>
    private Dictionary<int, ControlType> GetDropdownControlMapping()
    {
        Dictionary<int, ControlType> dropdownControlTypes = new Dictionary<int, ControlType>();
        dropdownControlTypes[0] = ControlType.Closed;
        dropdownControlTypes[1] = ControlType.AI;
        dropdownControlTypes[2] = ControlType.Player;
        return dropdownControlTypes;
    }
    
    /// <summary>
    /// Updates the control slots (<see cref="ControlSlotsScript.slots"/> with the set control changes.
    /// </summary>
    /// <param name="slot">The slot that changed.</param>
    /// <param name="value">The new value in the slot that changed.</param>
    private void ControlChange(int slot, int value)
    {
        Dictionary<int, ControlType> dropdownControlTypes = GetDropdownControlMapping();
        Debug.Log("Set player in slot " + slot + " as control type " + dropdownControlTypes[value]);
        controlSlots[slot].controlType = dropdownControlTypes[value];
        UpdateCharacterImages();
    }

    /// <summary>
    /// Called when the start game button is clicked. Will assign the random characters now.
    /// </summary>
    public void StartButtonClicked()
    {
        Debug.Log("Characters selected are: " + JsonConvert.SerializeObject(controlSlots));
        SceneManager.LoadScene("LevelSelect");
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
            else if (String.IsNullOrEmpty(controlSlots[slot].chosenCharacter)) imageName = "CharacterImages/RandomWhiteBackground";
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
    }
}
using UnityEngine;
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

    void Start()
    {
        // If we came from the main menu, we must be the host. Otherwise we're a client. This is the
        // easiest way to check this since the host might not be setup yet and hence we don't know if
        // we're a host or client (and the player objects might not have been spawned).
        if (backButtonTarget == "MainMenu")
        {
            // We're the host
            GameObject.Find("NetworkManager").GetComponent<NetworkManagerScript>().ConnectHost();
        }
        else
        {
            // Non-hosts need to disable control dropdowns
            GameObject.Find("P1Control").GetComponent<Dropdown>().enabled = false;
            GameObject.Find("P2Control").GetComponent<Dropdown>().enabled = false;
            GameObject.Find("P3Control").GetComponent<Dropdown>().enabled = false;
        }

        // Host must be p0, so disable the select there
        GameObject.Find("P0Control").GetComponent<Dropdown>().enabled = false;
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

        var controlSlotsObj = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>();
        controlSlotsObj.slots[slot].controlType = dropdownControlTypes[value];
        Debug.Log("Player " + slot + " now has control type " + controlSlotsObj.slots[slot].controlType);

        UpdateCharacterImages();
    }

    /// <summary>
    /// Updates all the character images based on the data in the control slots (ie, <see cref="ControlSlotsScript.slots"/>).
    /// </summary>
    private void UpdateCharacterImages()
    {
        var controlSlotsObj = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>();
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
            if (controlSlotsObj.slots[slot].controlType == ControlType.Closed) imageName = "CharacterImages/EmptySlot";
            else if (controlSlotsObj.slots[slot].chosenCharacter == null) imageName = "CharacterImages/UnknownCharacter";
            else imageName = "CharacterImages/" + controlSlotsObj.slots[slot].chosenCharacter;

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
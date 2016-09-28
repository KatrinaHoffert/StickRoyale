using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;

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
        if (backButtonTarget == "MainMenu")
        {
            // We're the host
            GameObject.Find("NetworkManager").GetComponent<NetworkManagerScript>().ConnectHost();
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
    /// For some stupid reason, Unity provides no way to change a dropdown value without the
    /// "on change" event firing. And that screws things up, so we have to effectively disable 
    /// the on change event's work when we're programmatically changing the dropdowns.
    /// </summary>
    private bool programmaticControlChangesOccurring = false;

    /// <summary>
    /// Updates the control slots (<see cref="ControlSlotsScript.slots"/> with the set control changes as well
    /// as performs some checking. Particularly, there must always be a host.
    /// </summary>
    /// <param name="slot">The slot that changed.</param>
    /// <param name="value">The new value in the slot that changed.</param>
    private void ControlChange(int slot, int value)
    {
        if (programmaticControlChangesOccurring) return;

        var controlSlotsObj = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>();

        // There must be a host, so if unset, re-set it.
        if(controlSlotsObj.slots[slot].controlType == ControlType.Host)
        {
            Debug.Log("Tried to unset 'host' in slot " + slot + ". Aborting.");
            programmaticControlChangesOccurring = true;
            GameObject.Find("P" + slot + "Control").GetComponent<Dropdown>().value = (int)ControlType.Host;
            programmaticControlChangesOccurring = false;
            return;
        }
        
        // If we're setting it somewhere, it must exist somewhere else. Swap those controls.
        if(value == (int)ControlType.Host)
        {
            // Figure out where the host currently is
            var allDropdowns = new GameObject[]
            {
                GameObject.Find("P0Control"),
                GameObject.Find("P1Control"),
                GameObject.Find("P2Control"),
                GameObject.Find("P3Control")
            };

            for(int dropdownSlot = 0; dropdownSlot < allDropdowns.Length; ++dropdownSlot)
            {
                var dropdown = allDropdowns[dropdownSlot].GetComponent<Dropdown>();
                if(dropdown.value == (int)ControlType.Host && dropdownSlot != slot)
                {
                    Debug.Log("Control " + slot + " is new host. Control " + dropdownSlot + " had host. Setting it to " + controlSlotsObj.slots[slot].controlType.ToString());
                    // Found the existing host -- give it the value of who we just made the host
                    programmaticControlChangesOccurring = true;
                    dropdown.value = (int)controlSlotsObj.slots[slot].controlType;
                    programmaticControlChangesOccurring = false;
                    controlSlotsObj.slots[dropdownSlot].controlType = controlSlotsObj.slots[slot].controlType;
                    break;
                }
            }
        }

        // And update the internal slots
        controlSlotsObj.slots[slot].controlType = (ControlType)value;

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
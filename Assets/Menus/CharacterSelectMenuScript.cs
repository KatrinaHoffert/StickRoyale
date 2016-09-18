using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class CharacterSelectMenuScript : MonoBehaviour
{
    private GameObject lastSelectedButton;

    void Start()
    {
        // Similar style to the main game script
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Setup the client
        GameObject.Find("NetworkManager").GetComponent<NetworkManagerScript>().ConnectHost();
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }
        else
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }
    }
    
    /// <summary>
    /// Called when the "back" button is pressed.
    /// </summary>
	public void BackButtonDown()
    {
        Debug.Log("Back");
        GameObject.Find("NetworkManager").GetComponent<NetworkManagerScript>().Disconnect();
        SceneManager.LoadScene("MainMenu");
    }
}

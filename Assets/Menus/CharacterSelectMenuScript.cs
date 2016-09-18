using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class CharacterSelectMenuScript : MonoBehaviour
{
    /// <summary>
    /// What scene the back button goes to (since we can arrive at this scene through either
    /// the new game or join game menus). Also used since which screen we came from determines
    /// whether or not we're a client.
    /// </summary>
    public static string backButtonTarget;

    private GameObject lastSelectedButton;

    void Start()
    {
        // Similar style to the main game script
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (backButtonTarget == "MainMenu")
        {
            // We're the host
            GameObject.Find("NetworkManager").GetComponent<NetworkManagerScript>().ConnectHost();
        }
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
        SceneManager.LoadScene(backButtonTarget);
    }
}

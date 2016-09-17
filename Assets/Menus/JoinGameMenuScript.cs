using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEditor;

public class JoinGameMenuScript : MonoBehaviour
{
    private GameObject lastSelectedButton;

    void Start()
    {
        // Similar style to the main game script
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
    /// Called when the "join game" button is pressed.
    /// </summary>
    public void JoinGameButtonDown()
    {
        Debug.Log("Join Game");
    }

    /// <summary>
    /// Called when the "back" button is pressed.
    /// </summary>
	public void BackButtonDown()
    {
        Debug.Log("Back");
        SceneManager.LoadScene("MainMenu");
    }
}

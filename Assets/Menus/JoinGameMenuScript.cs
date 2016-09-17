using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;

public class JoinGameMenuScript : MonoBehaviour
{
    /// <summary>
    /// The text input's object, supplied by Unity.
    /// </summary>
    public InputField hostIpInput;

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
        Debug.Log("Join Game of " + hostIpInput.text);
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

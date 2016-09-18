using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Keep track of what the currently selected object was so that if the mouse deselects it
    // (a behavior that annoyingly cannot be disabled!), we immediately reselect the last selected
    // button.
    private GameObject lastSelectedButton;

    void Start()
    {
        // Not using the mouse -- hide it. In the editor, press escape to unlock the mouse (necessary
        // so that you can stop playing the game.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Check if the mouse has deselected the selection
        if(EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }
        // Otherwise update the selection in case it changed
        else
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }
    }

    /// <summary>
    /// Called when the "new game" button is pressed.
    /// </summary>
	public void NewGameButtonDown()
    {
        Debug.Log("New Game");
        SceneManager.LoadScene("CharacterSelectMenu");
    }

    /// <summary>
    /// Called when the "join game" button is pressed.
    /// </summary>
    public void JoinGameButtonDown()
    {
        Debug.Log("Join Game");
        SceneManager.LoadScene("JoinGameMenu");
    }

    /// <summary>
    /// Called when the "quit" button is pressed. Note: doesn't work in the editor.
    /// </summary>
    public void QuitButtonDown()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}

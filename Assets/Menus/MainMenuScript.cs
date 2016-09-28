using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    /// <summary>
    /// Called when the "new game" button is pressed.
    /// </summary>
	public void NewGameButtonDown()
    {
        Debug.Log("New Game");
        CharacterSelectMenuScript.backButtonTarget = "MainMenu";
        SceneManager.LoadScene("CharacterSelectMenu");
    }

    /// <summary>
    /// Called when the "join game" button is pressed.
    /// </summary>
    public void JoinGameButtonDown()
    {
        Debug.Log("Join Game");
        CharacterSelectMenuScript.backButtonTarget = "JoinGameMenu";
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

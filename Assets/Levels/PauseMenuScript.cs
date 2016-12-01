using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the functionality of the pause menu (just a canvas).
/// </summary>
public class PauseMenuScript : MonoBehaviour
{
    public void ReturnToGameButtonClicked()
    {
        Time.timeScale = 1;
        GetComponent<Canvas>().enabled = false;
    }

    public void ExitToCharacterSelectButtonClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("CharacterSelectMenu");

        // Cleanup
        Destroy(GameObject.Find("Stats"));
    }

    public void ExitGameButtonClicked()
    {
        Application.Quit();
    }
}

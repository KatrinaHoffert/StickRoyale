using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Scripting for the PostGameScreen.
/// </summary>
public class PostGameScreenScript : MonoBehaviour
{
    void Start()
    {
        var stats = GameObject.Find("Stats").GetComponent<Stats>();
        var statsText = GameObject.Find("StatsText").GetComponent<Text>();

        string winnerLine = (stats.winner != "") ? "Winner is " + stats.winner + "!" : "No winner... :(";

        statsText.text = winnerLine;

        // TODO: Add more stats
    }

    public void NewGameButtonClicked()
    {
        // Cleanup first
        DestroyImmediate(GameObject.Find("ControlSlots"));
        DestroyImmediate(GameObject.Find("Stats"));

        SceneManager.LoadScene("CharacterSelectMenu");
    }

    public void ExitButtonClicked()
    {
        Debug.Log("Closing program");
        Application.Quit();
    }
}

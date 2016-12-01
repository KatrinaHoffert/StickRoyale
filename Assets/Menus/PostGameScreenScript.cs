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
        var winnerText = GameObject.Find("WinnerText").GetComponent<Text>();
        var statsText1 = GameObject.Find("StatsText1").GetComponent<Text>();
        var statsText2 = GameObject.Find("StatsText2").GetComponent<Text>();
        var controlSlots = CharacterSelectMenuScript.controlSlots;

        winnerText.text = (stats.winner != "") ? "Winner is " + stats.winner + "!" : "No winner... :(";
        winnerText.text += " - Game duration: " + FormatTime(stats.matchEndTime - stats.matchStartTime);

        // Note that the stats text is split into two columns for low resolution support
        for(int i = 0; i < controlSlots.Length; ++i)
        {
            if(controlSlots[i].controlType != ControlType.Closed)
            {
                if(i / 2 == 0)
                {
                    statsText1.text += "Player " + i + " (" + controlSlots[i].chosenCharacter + ")\n";
                    statsText1.text += "Kills: " + stats.kills[i] + "\n";
                    statsText1.text += "Falls: " + stats.falls[i] + "\n";
                    statsText1.text += "\n";
                }
                else
                {
                    statsText2.text += "Player " + i + " (" + controlSlots[i].chosenCharacter + ")\n";
                    statsText2.text += "Kills: " + stats.kills[i] + "\n";
                    statsText2.text += "Falls: " + stats.falls[i] + "\n";
                    statsText2.text += "\n";
                }
            }
        }
    }

    /// <summary>
    /// Formats a duration in seconds as a minutes and seconds timestamp (like "123:45").
    /// </summary>
    /// <param name="duration">The duration we want as a more readable timestamp.</param>
    /// <returns>A string for the timestamp.</returns>
    private string FormatTime(float duration)
    {
        return String.Format("{0}:{1:00}", (int)duration / 60, duration % 60);
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

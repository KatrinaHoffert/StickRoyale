using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostGameScreenScript : MonoBehaviour
{
    void Start()
    {
        // TODO: Set winner, stats
        var statsText = GameObject.Find("StatsText").GetComponent<Text>();
        statsText.text = "Winnah is...";
    }

    public void NewGameButtonClicked()
    {
        // TODO: Cleanup

        SceneManager.LoadScene("CharacterSelectMenu");
    }

    public void ExitButtonClicked()
    {
        Debug.Log("Closing program");
        Application.Quit();
    }
}

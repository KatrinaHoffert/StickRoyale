using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEngine.UI;

/// <summary>
/// Scripting for the LevelSelectMenu.
/// </summary>
public class LevelSelectScript : MonoBehaviour
{
    /// <summary>
    /// The names of all characters, used for randomly picking one.
    /// </summary>
    public readonly string[] characterOptions =
    {
        "Mage",
        "Rogue",
        "Knight"
    };

    public void Level1Clicked()
    {
        LoadLevel("Level6");
    }
    
    public void Level2Clicked()
    {
        LoadLevel("Level7");
    }

    public void Level3Clicked()
    {
        LoadLevel("level3");
    }

    void Start()
    {
        // Update the lives GUI in case we already changed this from having went back to the
        // character select
        var controlSlots = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>();
        GameObject.Find("LivesText").GetComponent<Text>().text = "Lives: " + controlSlots.playerLives;
    }

    /// <summary>
    /// Does the level loading, finalizing anything before the match can begin.
    /// </summary>
    /// <param name="name">The name of the level to load.</param>
    private void LoadLevel(string name)
    {
        var controlSlots = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>().slots;

        // Assign random characters for slots without a selected character
        for (int slot = 0; slot < controlSlots.Length; ++slot)
        {
            if (controlSlots[slot].controlType != ControlType.Closed)
            {
                if (string.IsNullOrEmpty(controlSlots[slot].chosenCharacter))
                {
                    controlSlots[slot].chosenCharacter = characterOptions[UnityEngine.Random.Range(0, characterOptions.Length)];
                }
            }
        }

        Debug.Log("Level chosen. Final players: " + JsonConvert.SerializeObject(controlSlots));
        SceneManager.LoadScene(name);
    }

    public void IncreaseLivesButtonClicked()
    {
        var controlSlots = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>();
        if (controlSlots.playerLives < 99) ++controlSlots.playerLives;
        GameObject.Find("LivesText").GetComponent<Text>().text = "Lives: " + controlSlots.playerLives;
    }

    public void DecreaseLivesButtonClicked()
    {
        var controlSlots = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>();
        if (controlSlots.playerLives > 1) --controlSlots.playerLives;
        GameObject.Find("LivesText").GetComponent<Text>().text = "Lives: " + controlSlots.playerLives;
    }

    public void BackButtonClicked()
    {
        SceneManager.LoadScene("CharacterSelectMenu");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

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

    public void BackButtonClicked()
    {
        SceneManager.LoadScene("CharacterSelectMenu");
    }
}

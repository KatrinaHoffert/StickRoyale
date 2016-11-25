using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// All the players, to avoid having to query for these unnecessarily.
    /// </summary>
    public static GameObject[] players;

    /// <summary>
    /// The spawn points that this level has.
    /// </summary>
    private GameObject[] spawnPoints;

    /// <summary>
    /// Stores the indicator objects for each player.
    /// </summary>
    private static GameObject[] indicators;

    /// <summary>
    /// Stores the hitbar objects for each player. The redbar is the object stored here and it has a child,
    /// greenbar, that shrinks as HP is lost.
    /// </summary>
    private static GameObject[] hitbars;
   
    void Start () {
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        // Create player objects
        var controlSlots = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>().slots;
        players = new GameObject[4];
        for(int i = 0; i < controlSlots.Length; ++i)
        {
            if(controlSlots[i].controlType != ControlType.Closed)
            {
                players[i] = Instantiate((GameObject)Resources.Load("CharacterPrefabs/" + controlSlots[i].chosenCharacter));

                // Name the objects for debugging
                if(controlSlots[i].controlType == ControlType.Player)
                {
                    players[i].name = "Player";
                    players[i].AddComponent<PlayerMovement>();
                }
                else
                {
                    players[i].name = "AI" + i;
                }

                // TODO: Attach player and AI scripts above
            }
        }

        // Player labels are prefabs, this is easier than to try to use the canvas to do this
        indicators = new GameObject[players.Length];

        // Hitbars are simply two sprites, a green box on top of a red box
        hitbars = new GameObject[players.Length];

        for (int i=0; i < players.Length; i++)
        {
            if (CharacterSelectMenuScript.controlSlots[i].controlType != ControlType.Closed)
            {
                // Placeholder image for the character
                players[i].AddComponent<SpriteRenderer>();
                players[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("CharacterImages/" + CharacterSelectMenuScript.controlSlots[i].chosenCharacter);

                // Puts players in spawn points (assumes there's at least 4 spawn points)
                players[i].transform.position = spawnPoints[i].transform.position;

                // Load the appropriate prefab for the player's indicator
                indicators[i] = Instantiate((GameObject)Resources.Load("player" + (i + 1)));
                indicators[i].transform.localScale = new Vector3(15, 15, 15);

                players[i].AddComponent<CharacterBase>();
                hitbars[i] = Instantiate((GameObject)Resources.Load("redbar"));
                hitbars[i].transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            }
        }
    }

    /// <summary>
    /// Respawns if player is dead.
    /// </summary>
    /// <param name="player">The player to check if we should respawn.</param>
    void RespawnIfDead(GameObject player)
    {
        var playerBase = player.GetComponent<CharacterBase>();
        if (playerBase.currentHitpoints <= 0)
        {
            playerBase.currentHitpoints = playerBase.maxHitpoints;
            player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length - 1)].transform.position;
            player.GetComponent<Rigidbody2D>().velocity = new Vector2();
        }
    }

    /// <summary>
    /// Updates the canvas element displaying the player stats.
    /// </summary>
    public void updateStats()
    {
        Text status = GameObject.Find("Text").GetComponent<Text>();
        
        status.text = "";
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null) continue;

            string characterName = CharacterSelectMenuScript.controlSlots[i].chosenCharacter;
            int playerHp = players[i].GetComponent<CharacterBase>().currentHitpoints;
            int playerMaxHp = players[i].GetComponent<CharacterBase>().maxHitpoints;
            status.text += "player " + (i + 1)+ "\n" + characterName + "\n" + playerHp + "/" + playerMaxHp + "\n";
        }
    }
    
    void FixedUpdate () {
        updateStats();

        for(int i = 0; i < players.Length; i++)
        {
            if (players[i] == null) continue;

            RespawnIfDead(players[i]);

            // Update player label position
            indicators[i].transform.position = players[i].transform.position + new Vector3(0, 20, 0);

            // Update hitbar location and size
            int playerHp = players[i].GetComponent<CharacterBase>().currentHitpoints;
            int playerMaxHp = players[i].GetComponent<CharacterBase>().maxHitpoints;
            hitbars[i].transform.position = players[i].transform.position + new Vector3(-10, 10, 0);
            hitbars[i].transform.FindChild("greenbar").transform.localScale = new Vector3((playerHp / (float)playerMaxHp), 1, 1);
        }
	}
}

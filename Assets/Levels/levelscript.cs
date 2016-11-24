using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class levelscript : NetworkBehaviour
{
    /// <summary>
    /// All the players, to avoid having to query for these unnecessarily.
    /// </summary>
    public static GameObject[] players;

    /// <summary>
    /// The spawn points that this level has.
    /// </summary>
    private NetworkStartPosition[] spawnPoints;

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
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        players = GameObject.FindGameObjectsWithTag("Player");

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
                
                players[i].GetComponent<testPlayerScript>().hitpoints = 100;
                hitbars[i] = Instantiate((GameObject)Resources.Load("redbar"));
                hitbars[i].transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            }
        }
    }

    /// <summary>
    /// Respawns if player is dead.
    /// </summary>
    /// <param name="player">The player to check if we should respawn.</param>
    void respawnIfDead(GameObject player)
    {
        if (player.GetComponent<testPlayerScript>().hitpoints <= 0)
        {
            player.GetComponent<testPlayerScript>().hitpoints = 100;
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
            string characterName = CharacterSelectMenuScript.controlSlots[i].chosenCharacter;
            string playerId = players[i].GetComponent<PlayerBase>().uniquePlayerId;
            int playerHp = players[i].GetComponent<testPlayerScript>().hitpoints;
            status.text += "player " + (i + 1)+ "\n" + characterName + "\n" + playerId + "\n" + playerHp + "/100\n";
        }
    }
    
    void FixedUpdate () {
        updateStats();

        for(int i = 0; i < players.Length; i++)
        {
            respawnIfDead(players[i]);

            // Update player label position
            indicators[i].transform.position = players[i].transform.position + new Vector3(0, 20, 0);
            
            // Update hitbar location and size
            hitbars[i].transform.position = players[i].transform.position + new Vector3(-10, 10, 0);
            hitbars[i].transform.FindChild("greenbar").transform.localScale = new Vector3((players[i].GetComponent<testPlayerScript>().hitpoints / 100.0f), 1, 1);
        }
	}
}

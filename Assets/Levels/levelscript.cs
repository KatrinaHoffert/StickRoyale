using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class levelscript : NetworkBehaviour {

    public static GameObject [] players;
    private NetworkStartPosition[] spawnPoints;
    private static GameObject[] indicators;
    private static GameObject[] hitbars;
    private int[] hitpoints = new int[4];
   
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

                // greenbar is a child of redbar, it shrinks to reveal redbar as you get damaged
                hitbars[i] = Instantiate((GameObject)Resources.Load("redbar"));
                hitbars[i].transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            }
        }
    }

    /// <summary>
    /// Respawns if player is dead.
    /// </summary>
    /// <param name="player">The player to check if we should respawn.</param>
    void respawn(GameObject player)
    {
        if (player.GetComponent<testPlayerScript>().hitpoints <= 0)
        {
            player.GetComponent<testPlayerScript>().hitpoints = 100;
            player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length - 1)].transform.position;
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

        for(int i=0; i < players.Length; i++)
        {

            //respawn player if player is dead
            respawn(players[i]);
            //move the player label to be above the appropriate player 
            indicators[i].transform.position = players[i].transform.position + new Vector3(0, 20, 0);
            //move the player hitbar to be above the appropriate player
            hitbars[i].transform.position = players[i].transform.position + new Vector3(-10, 10, 0);
            //scales the green bar currenthp/maxhp to reveal the red bar to create a hitbar
            hitbars[i].transform.FindChild("greenbar").transform.localScale = new Vector3((players[i].GetComponent<testPlayerScript>().hitpoints / 100.0f), 1, 1);
        }
	}
}

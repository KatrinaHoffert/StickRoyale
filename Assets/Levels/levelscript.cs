using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

//<<<<<<< HEAD
public class levelscript : NetworkBehaviour {

    public static GameObject [] players;
    private NetworkStartPosition[] spawnPoints;
    private static GameObject[] indicators;
    private static GameObject[] hitbars;




//=======
//public class levelscript : MonoBehaviour {
    public Sprite sprites;
    private int[] hitpoints = new int[4];

    public GameObject knight;
   
//>>>>>>> refs/remotes/origin/master

    // Use this for initialization
    void Start () {
        //finds all objects of type networkstartposition and adds them to an array, this is better than hardcoding a different script for each level
        //because you just neeed to add objects of type networkstartposition in each new level to make new spawnpoints
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        //get all objects of type player and adds them to an array
        players = GameObject.FindGameObjectsWithTag("Player");

        

        //player labels are prefabs, this is easier than to try to use the canvas to do this
        indicators = new GameObject[players.Length];
        //hitbars are simply two sprites, a green box on top of a red box
        hitbars = new GameObject[players.Length];
        for (int i=0; i<players.Length; i++)
        {
            // if there is no player (control type == 0) than this does not do anything
            if (CharacterSelectMenuScript.controlSlots[i].controlType != 0)
            {

                //initializePlayer(players[i]);

                players[i].AddComponent<SpriteRenderer>();
                players[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("CharacterImages/" + CharacterSelectMenuScript.controlSlots[i].chosenCharacter) ;
                //puts players in spawn points
                players[i].transform.position = spawnPoints[i].transform.position;
                //initial hp is 100
                players[i].GetComponent<testPlayerScript>().hitpoints = 100;
                //displays hp, id and for all players 
                Text status = GameObject.Find("Text").GetComponent<Text>();
               // status.text = status.text + "player "+ (i+1) + "\n"+ CharacterSelectMenuScript.controlSlots[i].chosenCharacter +"\n" + players[i].GetComponent<PlayerBase>().uniquePlayerId + "\n" + players[i].GetComponent<testPlayerScript>().hitpoints + "/100\n" ;
                //player indicators are loaded

                if (i == 0)
                {
                    indicators[i] = Instantiate((GameObject)Resources.Load("player1"));
                }
                else if (i == 1)
                {
                    indicators[i] = Instantiate((GameObject)Resources.Load("player2"));
                }
                else if (i == 2)
                {
                    indicators[i] = Instantiate((GameObject)Resources.Load("player3"));
                }
                else if (i == 3)
                {
                    indicators[i] = Instantiate((GameObject)Resources.Load("player4"));
                }

               //greenbar is a child of redbar, it shrinks to reveal redbar as you get damaged
                hitbars[i] = Instantiate((GameObject)Resources.Load("redbar"));
            

            }
                        
        }
    
        
	}
    /// <summary>
    /// respawns if player is dead
    /// </summary>
    /// <param name="player"></param>
    
    void respawn(GameObject player)
    {
        if (player.GetComponent<testPlayerScript>().hitpoints <= 0)
        {
            player.GetComponent<testPlayerScript>().hitpoints = 100;
            //placed in random spawnpoint
            player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length - 1)].transform.position;
        }
    }

    //updates the canvas element displaying the player stats
    public void updateStats()
    {
        Text status = GameObject.Find("Text").GetComponent<Text>();
        
        status.text = "";
        for (int i = 0; i < players.Length; i++)
        {
            status.text = status.text + "player " + (i + 1)+ "\n" + CharacterSelectMenuScript.controlSlots[i].chosenCharacter + "\n" + players[i].GetComponent<PlayerBase>().uniquePlayerId + "\n" + players[i].GetComponent<testPlayerScript>().hitpoints + "/100\n";
        }
    }
	
	// Update is called once per frame

	void FixedUpdate () {
        updateStats();
        for(int i=0; i<players.Length; i++)
        {

            //respawn player if player is dead
            respawn(players[i]);
            //move the player label to be above the appropriate player 
            indicators[i].transform.position = players[i].transform.position + new Vector3( 0,90, 0);
            //move the player hitbar to be above the appropriate player
            hitbars[i].transform.position = players[i].transform.position + new Vector3(-40, 30, 0);
            //scales the green bar currenthp/maxhp to reveal the red bar to create a hitbar
            hitbars[i].transform.FindChild("greenbar").transform.localScale = new Vector3((players[i].GetComponent<testPlayerScript>().hitpoints / 100), 1,1);
            
        }
        
//=======
	//void Update () {
	
	}
    /// <summary>
    /// Add parts to the player prefab 
    /// </summary>
    /// <param name="player"></param>
    void initializePlayer(GameObject player)
    {
        GameObject newplayer = (GameObject) Instantiate(knight, player.gameObject.transform.position, player.transform.rotation);
        player.transform.parent = newplayer.transform;
        /*
        player.gameObject.AddComponent<Rigidbody2D>();
        player.gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
        player.gameObject.AddComponent<SpriteRenderer>();
        player.gameObject.GetComponent<SpriteRenderer>().sprite = sprites;
        player.gameObject.AddComponent<BoxCollider2D>();*/
        player.gameObject.AddComponent<MovementScript>();
        player.gameObject.AddComponent<PlayerMovement>();
//>>>>>>> refs/remotes/origin/master
    }
}

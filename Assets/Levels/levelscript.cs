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
    //private GameObject [] players = new GameObject[4];
    //private NetworkStartPosition[] spawnPoints;
    private int[] hitpoints = new int[4];

    public GameObject knight;
   
//>>>>>>> refs/remotes/origin/master

    // Use this for initialization
    void Start () {
        //CharacterSelectMenuScript.controlSlots[0].networkPlayerId
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        players = GameObject.FindGameObjectsWithTag("Player");
        indicators = new GameObject[players.Length];
        hitbars = new GameObject[players.Length];
        for (int i=0; i<players.Length; i++)
        {

            if (CharacterSelectMenuScript.controlSlots[i].controlType != 0)
            {

                initializePlayer(players[i]);

                //puts players in spawn points
                players[i].transform.position = spawnPoints[i].transform.position;
                //initial hp is 100
                players[i].GetComponent<testPlayerScript>().hitpoints = 100;
                //displays hp, id and for all players 
                Text status = GameObject.Find("Text").GetComponent<Text>();
                status.text = status.text + "player "+ (i+1) + "\n" + players[i].GetComponent<PlayerBase>().uniquePlayerId + "\n" + players[i].GetComponent<testPlayerScript>().hitpoints + "/100\n" ;
                //player indicators

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
            player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length - 1)].transform.position;
        }
    }


    public void updateStats()
    {
        Text status = GameObject.Find("Text").GetComponent<Text>();
        
        status.text = "";
        for (int i = 0; i < players.Length; i++)
        {
            status.text = status.text + "player " + (i + 1) + "\n" + players[i].GetComponent<PlayerBase>().uniquePlayerId + "\n" + players[i].GetComponent<testPlayerScript>().hitpoints + "/100\n";
        }
    }
	
	// Update is called once per frame
//<<<<<<< HEAD
	void FixedUpdate () {
        updateStats();
        for(int i=0; i<players.Length; i++)
        {


            respawn(players[i]);
            indicators[i].transform.position = players[i].transform.position + new Vector3( 0,90, 0);
            hitbars[i].transform.position = players[i].transform.position +new  Vector3(-40,30,0);
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

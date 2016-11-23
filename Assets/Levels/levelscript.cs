using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class levelscript : MonoBehaviour {
    public Sprite sprites;
    private GameObject [] players = new GameObject[4];
    private NetworkStartPosition[] spawnPoints;
    private int[] hitpoints = new int[4];

    public GameObject knight;
   

    // Use this for initialization
    void Start () {
        //CharacterSelectMenuScript.controlSlots[0].networkPlayerId
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        players = GameObject.FindGameObjectsWithTag("Player");
        for (int i=0; i<players.Length; i++)
        {

            if (CharacterSelectMenuScript.controlSlots[i].controlType != 0)
            {

                initializePlayer(players[i]);

                //puts players in spawn points
                players[i].transform.position = spawnPoints[i].transform.position;
                hitpoints[i] = 100;
                //status[i] = new GameObject();
                //status[i].AddComponent<Text>();
                //status[i].transform.position = new Vector2(i * 200 - 400, 400);

                //status[i] = 
                //Text status = GameObject.Find("Text").GetComponent<Text>();
                 //status.text = players[i].GetComponent<PlayerBase>().playerName + "\n" + players[i].GetComponent<PlayerBase>().uniquePlayerId + "\n" + hitpoints[i] + "/100";
                
                //GameObject.Find("Canvas").

            }
                        
        }

        
	}
	
	// Update is called once per frame
	void Update () {
	
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
    }
}

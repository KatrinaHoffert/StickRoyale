using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class levelscript : MonoBehaviour {
    public Sprite sprites;
    private GameObject [] players = new GameObject[4];
    private NetworkStartPosition[] spawnPoints;
    private int[] hitpoints = new int[4];
   

    // Use this for initialization
    void Start () {
        //CharacterSelectMenuScript.controlSlots[0].networkPlayerId
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        players = GameObject.FindGameObjectsWithTag("Player");
        for (int i=0; i<players.Length; i++)
        {

            if (CharacterSelectMenuScript.controlSlots[i].controlType != 0)
            {
				players[i].gameObject.AddComponent<Rigidbody2D>();
                players[i].gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
				players[i].gameObject.AddComponent<SpriteRenderer>();
				players[i].gameObject.GetComponent<SpriteRenderer>().sprite = sprites;
				players[i].gameObject.AddComponent<BoxCollider2D>();
				players[i].gameObject.AddComponent<MovementScript>();
			    players[i].gameObject.AddComponent<PlayerMovement>();


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
}

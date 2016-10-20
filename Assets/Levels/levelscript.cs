using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class levelscript : NetworkBehaviour {

    public static GameObject [] players;
    private NetworkStartPosition[] spawnPoints;
    private static GameObject[] indicators;
    private static GameObject[] hitbars;





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

               //puts players in spawn points
                players[i].transform.position = spawnPoints[i].transform.position;
                players[i].GetComponent<testPlayerScript>().hitpoints = 100;
                Text status = GameObject.Find("Text").GetComponent<Text>();
                status.text = status.text + "player "+ (i+1) + "\n" + players[i].GetComponent<PlayerBase>().uniquePlayerId + "\n" + players[i].GetComponent<testPlayerScript>().hitpoints + "/100\n" ;
                indicators[i] = Instantiate((GameObject)Resources.Load("Text"));
                indicators[i].GetComponent<Text>().text = "player " + (i+1);
                indicators[i].transform.SetParent(GameObject.Find("Canvas").transform);
                hitbars[i] = Instantiate((GameObject)Resources.Load("redbar"));
            

            }
                        
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
	void FixedUpdate () {
        updateStats();
        for(int i=0; i<players.Length; i++)
        {
            indicators[i].transform.position = players[i].transform.position + new Vector3(300,250,0);
            hitbars[i].transform.position = players[i].transform.position + new Vector3(0,30,0);
            hitbars[i].transform.FindChild("greenbar").transform.localScale = new Vector3((players[i].GetComponent<testPlayerScript>().hitpoints / 100), 1,1);
            
        }
        
    }
}

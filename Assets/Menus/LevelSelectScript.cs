using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class LevelSelectScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    /// <summary>
    /// level 1 is selected
    /// </summary>
    public void level1selected()
    {
        //NetworkManger.
        // SceneManager.LoadScene("Level1");
        NetworkManagerScript.getNetworkManager().GetComponent<NetworkManager>().ServerChangeScene("Level1");

    }

    /// <summary>
    /// level 2 is selected
    /// </summary>
    public void level2selected()
    {

        //SceneManager.LoadScene("Level2");
        NetworkManagerScript.getNetworkManager().GetComponent<NetworkManager>().ServerChangeScene("Level2");



    }


}


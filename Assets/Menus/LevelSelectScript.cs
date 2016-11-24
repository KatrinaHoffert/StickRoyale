using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class LevelSelectScript : MonoBehaviour
{
    public void Level1Selected()
    {
        NetworkManagerScript.GetNetworkManager().GetComponent<NetworkManager>().ServerChangeScene("Level1");
    }
    
    public void Level2Selected()
    {
        NetworkManagerScript.GetNetworkManager().GetComponent<NetworkManager>().ServerChangeScene("Level2");
    }
}

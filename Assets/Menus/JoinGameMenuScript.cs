using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class JoinGameMenuScript : MonoBehaviour
{
    /// <summary>
    /// The text input's object, supplied by Unity.
    /// </summary>
    public InputField hostIpInput;

    void Start()
    {
        // Only show the spinner when we're doing the loading (done in
        // NetworkManagerScript.ConnectClient).
        GameObject.Find("LoadingAnimation").GetComponent<SpriteRenderer>().enabled = false;
    }

    /// <summary>
    /// Called when the "join game" button is pressed. Initiates the process of connecting to the host
    /// and will transition to the character select menu upon completed connection.
    /// </summary>
    public void JoinGameButtonDown()
    {
        Debug.Log("Attempting to join game of " + hostIpInput.text);
        
        var ipAndPort = hostIpInput.text.Split(':');
        int port = ipAndPort.Length == 2 ? int.Parse(ipAndPort[1]) : 7777;

        GameObject.Find("NetworkManager").GetComponent<NetworkManagerScript>().ConnectClient(ipAndPort[0], port);
    }

    /// <summary>
    /// Called when the "back" button is pressed.
    /// </summary>
	public void BackButtonDown()
    {
        Debug.Log("Back");
        SceneManager.LoadScene("MainMenu");
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkManagerScript : MonoBehaviour
{
    public void ConnectHost()
    {
        // If we haven't already connected to a host, we become the host. Note that we might
        // already be the host!
        Debug.Log("Starting host");
        GetComponent<NetworkManager>().StartHost();
    }

    public void ConnectClient(string ip, int port)
    {
        Debug.Log("Connecting client");

        // Event callbacks
        NetworkManagerWithCallbacks.onClientConnectCallbacks.Add(conn =>
        {
            Debug.Log("Connection " + conn.address + " successful");
            SceneManager.LoadScene("CharacterSelectMenu");
        });
        NetworkManagerWithCallbacks.onClientErrorCallbacks.Add((conn, errorId) =>
        {
            Debug.Log("Connection " + conn.address + " failed, error " + errorId);
        });
        NetworkManagerWithCallbacks.onClientDisconnectCallbacks.Add(conn =>
        {
            Debug.Log("Connection " + conn.address + " disconnected");
        });

        GetComponent<NetworkManager>().networkAddress = ip;
        GetComponent<NetworkManager>().networkPort = port;
        GetComponent<NetworkManager>().StartClient();
        /*GetComponent<NetworkManager>().client.RegisterHandler(MsgType.Connect, (msg) =>
        {
            // Load the character select menu (where we'll trigger the 'ready' flag)
            Debug.Log("Connection successful");
            SceneManager.LoadScene("CharacterSelectMenu");
            ClientScene.Ready(NetworkClient.allClients[0].connection);
        });*/
    }

    public void Disconnect()
    {
        Debug.Log("Disconnecting");
        GetComponent<NetworkManager>().StopHost();
        GetComponent<NetworkManager>().StopClient();

        // Destroy all the players, too (since they're not normally destroyed on scene loading)
        foreach(var player in FindObjectsOfType<PlayerBase>())
        {
            DestroyImmediate(player.gameObject);
        }
    }
}

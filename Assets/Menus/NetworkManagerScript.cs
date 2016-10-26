using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking.NetworkSystem;

public class NetworkManagerScript : MonoBehaviour
{

    public static GameObject getNetworkManager()
    {
        return GameObject.Find("NetworkManager");
    }
    /// <summary>
    /// Sets up and initializes the host. 
    /// </summary>
    /// <param name="playerJoinedCallback">Will be called every time a player joins the game.</param>
    /// 
    public void ConnectHost(Action playerJoinedCallback)
    {
        NetworkManagerWithCallbacks.onServerAddPlayerCallbacks.Add((conn, id) => playerJoinedCallback());

        // If we haven't already connected to a host, we become the host. Note that we might
        // already be the host!
        Debug.Log("Starting host");
        GetComponent<NetworkManager>().StartHost();
        
    }

    /// <summary>
    /// Connects a client to the host running at the specified network location.
    /// </summary>
    /// <param name="ip">IP address of the host.</param>
    /// <param name="port">Port of the host.</param>
    public void ConnectClient(string ip, int port)
    {
        Debug.Log("Connecting client");
        GameObject.Find("LoadingAnimation").GetComponent<SpriteRenderer>().enabled = true;

        // Event callbacks
        NetworkManagerWithCallbacks.onClientConnectCallbacks.Add(conn =>
        {
            Debug.Log("Connection " + conn.address + " successful");
            SceneManager.LoadScene("CharacterSelectMenu");
        });
        NetworkManagerWithCallbacks.onClientErrorCallbacks.Add((conn, errorId) =>
        {
            // Not sure where this is used from -- never seen it hit yet
            Debug.Log("Connection " + conn.address + " failed, error " + errorId);
        });
        NetworkManagerWithCallbacks.onClientDisconnectCallbacks.Add(conn =>
        {
            Debug.Log("Connection " + conn.address + " disconnected");
            SceneManager.LoadScene("DisconnectedScreen");
        });

        GetComponent<NetworkManager>().networkAddress = ip;
        GetComponent<NetworkManager>().networkPort = port;
        GetComponent<NetworkManager>().StartClient();

        NetworkManager.singleton.client.RegisterHandler(CustomMessageTypes.PlayerSetUniqueId, OnUniquePlayerIdAssigned);
    }

    /// <summary>
    /// Called via message passing for the host to set this player's ID.
    /// </summary>
    /// <param name="netMsg">A <see cref="StringMessage"/> that contains simply the unique player ID that has
    /// been assigned to us.</param>
    public void OnUniquePlayerIdAssigned(NetworkMessage netMsg)
    {
        string id = netMsg.ReadMessage<StringMessage>().value;
        Debug.Log("Host has assigned us ID: " + id);
        Global.GetOurPlayer().uniquePlayerId = id;
    }

    /// <summary>
    /// Disconnects us (regardless of whether we're a host or client) and performs cleanup of networked objects.
    /// </summary>
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

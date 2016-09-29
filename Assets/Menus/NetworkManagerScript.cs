using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;

public class NetworkManagerScript : MonoBehaviour
{
    /// <summary>
    /// Sets up and initializes the host. 
    /// </summary>
    /// <param name="playerJoinedCallback">Will be called every time a player joins the game.</param>
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

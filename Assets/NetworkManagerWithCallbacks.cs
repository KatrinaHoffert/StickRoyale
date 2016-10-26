using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

/// <summary>
/// A version of the NetworkManager that allows us to handle various events, because for whatever inane
/// reason, the devs at Unity thought that this wasn't an important feature...
/// </summary>
public class NetworkManagerWithCallbacks : NetworkManager
{


    /// <summary>
    /// A list of functions that will be called when `OnClientConnect` fires.
    /// </summary>
    public static IList<Action<NetworkConnection>> onClientConnectCallbacks = new List<Action<NetworkConnection>>();

    /// <summary>
    /// A list of functions that will be called when `OnClientError` fires.
    /// </summary>
    public static IList<Action<NetworkConnection, int>> onClientErrorCallbacks = new List<Action<NetworkConnection, int>>();

    /// <summary>
    /// A list of functions that will be called when `OnClientDisconnect` fires.
    /// </summary>
    public static IList<Action<NetworkConnection>> onClientDisconnectCallbacks = new List<Action<NetworkConnection>>();

    /// <summary>
    /// A list of functions that will be called when `OnServerAddPlayer` fires.
    /// </summary>
    public static IList<Action<NetworkConnection, short>> onServerAddPlayerCallbacks = new List<Action<NetworkConnection, short>>();

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        foreach (var func in onClientConnectCallbacks) func(conn);
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);
        foreach (var func in onClientErrorCallbacks) func(conn, errorCode);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        foreach (var func in onClientDisconnectCallbacks) func(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        foreach (var func in onServerAddPlayerCallbacks) func(conn, playerControllerId);
    }
}

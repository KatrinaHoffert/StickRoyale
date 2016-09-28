using UnityEngine;
using System.Collections;
using System;

public class Global : MonoBehaviour {
    /// <summary>
    /// Method for determining if the active player is a client or not. Can only be called after the
    /// player objects have initialized.
    /// </summary>
    /// <returns>True if we are the client, false if we're the host.</returns>
    public static bool PlayerIsClient()
    {
        PlayerBase[] players = FindObjectsOfType<PlayerBase>();
        foreach (var player in players)
        {
            // Found us
            if(player.playerName == "You")
            {
                return !player.isServer;
            }
        }

        throw new Exception("No player object found for this player (something is very wrong or this was called too soon)");
    }
}

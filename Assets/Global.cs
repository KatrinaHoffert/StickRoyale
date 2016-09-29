using UnityEngine;
using System.Collections;
using System;

public class Global : MonoBehaviour {
    /// <summary>
    /// Gets the current player's <see cref="PlayerBase"/> object. To find the player object that
    /// this script is attached to, access the `gameObject` field of the `PlayerBase`.
    /// </summary>
    /// <returns>The controlling character's `PlayerBase`.</returns>
    public static PlayerBase GetOurPlayer()
    {
        PlayerBase[] players = FindObjectsOfType<PlayerBase>();
        foreach (var player in players)
        {
            // Found us
            if (player.playerName == "You")
            {
                return player;
            }
        }

        throw new Exception("No player object found for this player (something is very wrong or this was called too soon)");
    }

    /// <summary>
    /// Method for determining if the active player is a client or not. Can only be called after the
    /// player objects have initialized.
    /// </summary>
    /// <returns>True if we are the client, false if we're the host.</returns>
    public static bool PlayerIsClient()
    {
        return !GetOurPlayer().isServer;
    }
}

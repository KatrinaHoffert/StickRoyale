using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class PlayerBase : NetworkBehaviour
{
    /// <summary>
    /// A string that identifies the player by their slot (eg, "p1", "p2", etc). But for
    /// the local player, it'll be "You".
    /// </summary>
    public string playerName;

    /// <summary>
    /// A unique ID for this player. Generated after the player is fully connected (or when the
    /// host is initialized).
    /// </summary>
    public string uniquePlayerId;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartLocalPlayer()
    {
        // A crucial aspect for identifying our own player object.
        playerName = "You";
    }
}

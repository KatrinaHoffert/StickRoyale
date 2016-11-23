﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class PlayerBase : NetworkBehaviour
{
    /// <summary>
    /// Currently used simply to identify the local player as "You". In the future, could be used to get
    /// a friendly name for network players. But for now, all other players will have this blank.
    /// Note that a lot of code depends on this being "You" so that we can identify which of the
    /// PlayerBase objects is ours.
    /// </summary>
    public string playerName;

    /// <summary>
    /// A unique ID for this player. Generated by the host after the player is fully connected and
    /// before it's assigned a slot. Set by <see cref="NetworkManagerScript.OnUniquePlayerIdAssigned"/>, which
    /// the host remotely calls in <see cref="CharacterSelectMenuScript.AssignUnassignedPlayersToNetworkSlots"/>.
    /// Should happen very quickly, but not instantly after the Player object is created. So must expect that this
    /// can be null during the initial connection period.
    /// </summary>
    [SyncVar]
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

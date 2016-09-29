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
        uniquePlayerId = Guid.NewGuid().ToString();
    }

    public override void OnStartLocalPlayer()
    {
        playerName = "You";

        // If we're not the host, set us to a network slot
        if (!isServer)
        {
            var controlSlotsObj = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>();
            foreach(var slot in controlSlotsObj.slots)
            {
                if(slot.controlType == ControlType.Network && slot.networkPlayerId == null)
                {
                    slot.networkPlayerId = uniquePlayerId;
                    return;
                }
            }
        }
    }
}

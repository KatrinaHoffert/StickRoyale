using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerBase : NetworkBehaviour
{
    /// <summary>
    /// A string that identifies the player by their slot (eg, "p1", "p2", etc). But for
    /// the local player, it'll be "You".
    /// </summary>
    public string playerName;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartLocalPlayer()
    {
        playerName = "You";
    }
}

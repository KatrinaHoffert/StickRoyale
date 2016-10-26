using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Custom message types (an extension of the types from <see cref="MsgType"/>).
/// </summary>
public class CustomMessageTypes
{
    /// <summary>
    /// Used for the host to set the unique ID of a player.
    /// </summary>
    public static short PlayerSetUniqueId = MsgType.Highest + 1;
}

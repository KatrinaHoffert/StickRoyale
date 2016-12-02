using System;
using UnityEngine;

/// <summary>
/// A record of a powerup and when it was picked up.
/// </summary>
class PowerupRecord
{
    /// <summary>
    /// The powerup we picked up.
    /// </summary>
    public PowerupBase powerup;

    /// <summary>
    /// The game time that it was acquired.
    /// </summary>
    public float pickupTime;

    public PowerupRecord(PowerupBase powerup)
    {
        this.powerup = powerup;
        pickupTime = Time.time;
    }
}

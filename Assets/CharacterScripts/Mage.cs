using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Mage specific behavior.
/// </summary>
public class Mage : CharacterBase
{
    protected override void Cleanup()
    {
        // Remove any projectiles that this character cast
        var projectile1s = FindObjectsOfType<MageAttack1Trigger>().Where(trigger => trigger.casterObject == gameObject);
        var projectile2s = FindObjectsOfType<MageAttack2Trigger>().Where(trigger => trigger.casterObject == gameObject);
        foreach (var projectile in projectile1s) Destroy(projectile);
        foreach (var projectile in projectile2s) Destroy(projectile);
    }
}
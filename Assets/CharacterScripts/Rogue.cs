using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Rogue specific behavior.
/// </summary>
public class Rogue : CharacterBase
{
    protected override void Cleanup()
    {
        // Ensure that our attack AoEs are disabled
        Transform[] childTransforms = GetComponentsInChildren<Transform>();
        foreach (Transform transform in childTransforms)
        {
            if (transform.tag == "Attack1") transform.GetComponent<Collider2D>().enabled = false;
            if (transform.tag == "Attack2") transform.GetComponent<Collider2D>().enabled = false;
        }
    }
}
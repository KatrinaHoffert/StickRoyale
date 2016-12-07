using UnityEngine;
using System.Collections;

/// <summary>
/// Collision handling for when characters stack on someone's head.
/// </summary>
public class HeadCollision : MonoBehaviour
{
    PlayerMovement moveScript;
    
    /// <summary>
    /// Used for detecting if players stack.
    /// </summary>
    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            gameObject.GetComponentInParent<PlayerBase>().MoveOffPlayer();
        }
    }
}

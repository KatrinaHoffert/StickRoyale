using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MageAttack1Trigger : MonoBehaviour
{
    /// <summary>
    /// Pushback intensity of the collision.
    /// </summary>
    public float pushbackMagnitude = 100;
    
    /// <summary>
    /// Damage taken on collision.
    /// </summary>
    public int damage = 15;

    /// <summary>
    /// Players that have taken damage from this effect (so they can't be double tapped).
    /// </summary>
    private List<GameObject> playersAlreadyHit = new List<GameObject>();

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player") && !playersAlreadyHit.Contains(coll.gameObject))
        {
            int direction = GetComponent<SpriteRenderer>().flipX ? -1 : 1;
            coll.gameObject.GetComponent<CharacterBase>().Damage(damage);
            coll.gameObject.GetComponent<CharacterBase>().DamageForce(transform.right * pushbackMagnitude * direction);
            playersAlreadyHit.Add(coll.gameObject);
        }
    }
}

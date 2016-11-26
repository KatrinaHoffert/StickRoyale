using UnityEngine;
using System.Collections;

public class KnightAttack2Trigger : MonoBehaviour
{
    /// <summary>
    /// Pushback intensity of the collision.
    /// </summary>
    public float pushbackMagnitude = 125;

    /// <summary>
    /// Damage taken on collision.
    /// </summary>
    public int damage = 20;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            // Don't hurt ourselves
            if (coll.gameObject == transform.parent.gameObject) return;

            int direction = transform.parent.GetComponent<CharacterBase>().facing;
            coll.gameObject.GetComponent<CharacterBase>().Damage(damage);
            coll.gameObject.GetComponent<CharacterBase>().DamageForce(new Vector2(direction, 0.5f) * pushbackMagnitude);
        }
    }
}

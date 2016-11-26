using UnityEngine;
using System.Collections;

public class RogueAttack2Trigger : MonoBehaviour
{
    /// <summary>
    /// Pushback intensity of the collision.
    /// </summary>
    public float pushbackMagnitude = 150;

    /// <summary>
    /// Damage taken on collision.
    /// </summary>
    public int damage = 20;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            int direction = transform.parent.GetComponent<CharacterBase>().facing;
            coll.gameObject.GetComponent<CharacterBase>().Damage(damage);
            coll.gameObject.GetComponent<CharacterBase>().DamageForce(Vector3.right * direction * pushbackMagnitude);
        }
    }
}

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

    private Stats stats;

    void Start()
    {
        stats = GameObject.Find("Stats").GetComponent<Stats>();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            // Don't hurt ourselves
            if (coll.gameObject == transform.parent.gameObject) return;

            int direction = transform.parent.gameObject.GetComponent<CharacterBase>().facing;
            var targetCharacterBase = coll.gameObject.GetComponent<CharacterBase>();
            targetCharacterBase.Damage(damage);
            targetCharacterBase.DamageForce(new Vector2(direction, 0.75f) * pushbackMagnitude);

            if (targetCharacterBase.currentHitpoints <= 0) stats.AddKill(transform.parent.gameObject);
        }
    }
}

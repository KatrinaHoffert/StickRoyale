using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    /// <summary>
    /// Players that have taken damage from this effect (so they can't be double tapped).
    /// </summary>
    public List<GameObject> playersAlreadyHit = new List<GameObject>();

    private Stats stats;

    void Start()
    {
        stats = GameObject.Find("Stats").GetComponent<Stats>();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player") && !playersAlreadyHit.Contains(coll.gameObject))
        {
            // Don't hurt ourselves
            if (coll.gameObject == transform.parent.gameObject) return;

            int direction = transform.parent.gameObject.GetComponent<CharacterBase>().facing;
            var targetCharacterBase = coll.gameObject.GetComponent<CharacterBase>();
            targetCharacterBase.Damage(damage);
            targetCharacterBase.DamageForce(new Vector2(direction, 0.75f) * pushbackMagnitude);
            playersAlreadyHit.Add(coll.gameObject);

            if (targetCharacterBase.currentHitpoints <= 0) stats.AddKill(transform.parent.gameObject);
        }
    }
}

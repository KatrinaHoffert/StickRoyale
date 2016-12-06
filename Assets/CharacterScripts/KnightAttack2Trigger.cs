using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnightAttack2Trigger : MonoBehaviour
{
    /// <summary>
    /// Pushback intensity of the collision.
    /// </summary>
    public float pushbackMagnitude = 125;

    /// <summary>
    /// Damage taken on collision.
    /// </summary>
    public int damage = 15;

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

            var attackerCharacter = transform.parent.gameObject.GetComponent<CharacterBase>();
            int direction = attackerCharacter.facing;
            var targetCharacterBase = coll.gameObject.GetComponent<CharacterBase>();
            targetCharacterBase.Damage((int)(damage * attackerCharacter.damageMultiplier));
            targetCharacterBase.DamageForce(new Vector2(direction, 0.5f) * pushbackMagnitude);
            playersAlreadyHit.Add(coll.gameObject);
            Debug.Log(coll.gameObject.ToString() + "Hit");

            ///burns target if the fire powerup is active
            if (attackerCharacter.onFire)
            {
                targetCharacterBase.burning = 0;
            }

            if (targetCharacterBase.currentHitpoints <= 0) stats.AddKill(transform.parent.gameObject);
        }
    }
}

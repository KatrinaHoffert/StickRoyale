using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A base to all attack triggers, which are essentially the area of effect for attacks. Colliding
/// with the trigger area implies you were hit by the attack. There's two main ways that these could
/// be structured:
/// 
///  1. The knight and rogue have the attack triggers as child objects that are in front of the
///     character, since their attacks are melee and move with the character. These colliders are
///     enabled and disabled when they attacks go on.
///  2. The mage spawns prefabs with its attack triggers. This lets the attacks be independent of the
///     character. Also means that the means of getting the attacker object is different.
///     
/// Default implementation is based on the knight/rogue pattern, with the mage (and any others?) overriding
/// appropriate functionality.
/// 
/// All triggers must implement <see cref="GetDamage"/> and <see cref="GetDamageForce"/>.
/// 
/// The triggers will also keep track of the number of kills.
/// </summary>
public abstract class AttackTriggerBase : MonoBehaviour
{
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
            var attackerCharacter = GetAttackerCharacterBase();

            // Don't hurt ourselves
            if (coll.gameObject == attackerCharacter.gameObject) return;

            int direction = GetAttackDirection(attackerCharacter);
            var targetCharacterBase = coll.gameObject.GetComponent<CharacterBase>();
            targetCharacterBase.Damage((int)(GetDamage() * attackerCharacter.damageMultiplier));
            targetCharacterBase.DamageForce(GetDamageForce() * direction);
            playersAlreadyHit.Add(coll.gameObject);
            
            if (attackerCharacter.onFire)
            {
                targetCharacterBase.SetOnFire();
            }

            if (targetCharacterBase.currentHitpoints <= 0) stats.AddKill(attackerCharacter.gameObject);
        }

        PostCollisionTrigger();
    }

    /// <summary>
    /// Base amout of damage that the attack deals.
    /// </summary>
    protected abstract int GetDamage();

    /// <summary>
    /// Direction and magnitude of damage force (ie, the knockback). This will be multiplied
    /// by the direction from <see cref="GetAttackDirection"/>.
    /// </summary>
    protected abstract Vector2 GetDamageForce();

    /// <summary>
    /// Returns the attacker's <see cref="CharacterBase"/>. Default implementation is for the
    /// knight and rogue method of attack triggers.
    /// </summary>
    protected virtual CharacterBase GetAttackerCharacterBase()
    {
        return transform.parent.gameObject.GetComponent<CharacterBase>();
    }

    /// <summary>
    /// Gets the direction that the attack is in. Default implementation assumes that the
    /// direction of the attack is the direction of the attacker.
    /// </summary>
    /// <param name="attackerCharacter">The attacker's character.</param>
    /// <returns>-1 for facing left, 1 for facing right.</returns>
    protected virtual int GetAttackDirection(CharacterBase attackerCharacter)
    {
        return attackerCharacter.facing;
    }

    /// <summary>
    /// Called at the end of the collision, always.
    /// </summary>
    protected virtual void PostCollisionTrigger() { }
}

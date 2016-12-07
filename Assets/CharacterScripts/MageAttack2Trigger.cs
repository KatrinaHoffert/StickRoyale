using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MageAttack2Trigger : AttackTriggerBase
{
    /// <summary>
    /// The game object of the caster, who we don't want getting hurt by their own attack.
    /// </summary>
    public GameObject casterObject;

    /// <summary>
    /// The velocity of the projectile.
    /// </summary>
    public float velocity = 4.0f;

    /// <summary>
    /// Maximum distance the projectile will travel before being destroyed.
    /// </summary>
    public float maxDistance = 100;

    /// <summary>
    /// The direction that the projectile is moving in. -1 for left, 1 for right.
    /// </summary>
    private int direction;

    protected override int GetDamage()
    {
        return 20;
    }

    protected override Vector2 GetDamageForce()
    {
        return new Vector2(0.25f, 1.0f) * 100;
    }

    protected override CharacterBase GetAttackerCharacterBase()
    {
        return casterObject.GetComponent<CharacterBase>();
    }

    protected override int GetAttackDirection(CharacterBase attackerCharacter)
    {
        return direction;
    }

    protected override void PostCollisionTrigger()
    {
        // No matter what we hit, the projectile gets destroyed
        Destroy(gameObject);
    }

    void Start()
    {
        direction = GetComponent<SpriteRenderer>().flipX ? -1 : 1;
    }

    void Update()
    {
        transform.position = transform.position + new Vector3(velocity * Time.deltaTime * direction, 0);

        // Don't let the projectile last forever.
        if (Math.Abs(transform.position.x) > maxDistance) Destroy(gameObject);
    }
}

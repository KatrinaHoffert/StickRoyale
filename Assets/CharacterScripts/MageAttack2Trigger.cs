using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MageAttack2Trigger : MonoBehaviour
{
    /// <summary>
    /// Pushback intensity of the collision.
    /// </summary>
    public float pushbackMagnitude = 100;
    
    /// <summary>
    /// Damage taken on collision.
    /// </summary>
    public int damage = 20;

    /// <summary>
    /// The velocity of the projectile.
    /// </summary>
    public float velocity = 4.0f;

    /// <summary>
    /// Maximum distance the projectile will travel before being destroyed.
    /// </summary>
    public float maxDistance = 100;

    /// <summary>
    /// The game object of the caster, who we don't want getting hurt by their own attack.
    /// </summary>
    public GameObject casterObject;

    /// <summary>
    /// The direction that the projectile is moving in. -1 for left, 1 for right.
    /// </summary>
    private int direction;

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

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            // Don't hurt ourselves
            if (coll.gameObject == casterObject) return;
            
            coll.gameObject.GetComponent<CharacterBase>().Damage(damage);
            coll.gameObject.GetComponent<CharacterBase>().DamageForce(new Vector2(0.25f, direction) * pushbackMagnitude);
        }

        // No matter what we hit, the projectile gets destroyed
        Destroy(gameObject);
    }
}

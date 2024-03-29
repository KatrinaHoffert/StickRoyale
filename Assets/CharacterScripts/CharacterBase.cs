﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// A variety of features that are common to every character. Managing of HP and lives are the
/// most important here, but there's also some movement related stuff.
/// </summary>
public class CharacterBase : MonoBehaviour
{
    /// <summary>
    /// Max HP. When it hits zero, the character dies.
    /// </summary>
    public int maxHitpoints = 100;

    /// <summary>
    /// Current HP.
    /// </summary>
    public int currentHitpoints;

    /// <summary>
    /// Number of lives this player has.
    /// </summary>
    public int lives = 3;

    /// <summary>
    /// The direction that the character is facing. 1 for right, -1 for left.
    /// </summary>
    public int facing = 1;

    /// <summary>
    /// A multiplier to movement speed that this character has.
    /// </summary>
    public float movementSpeedMultiplier = 1.0f;

    /// <summary>
    /// A multiplier to all damage that this character deals.
    /// </summary>
    public float damageMultiplier = 1.0f;

    /// <summary>
    /// If true, we can pass on the burning negative status with our attacks.
    /// </summary>
    public bool onFire = false;

    /// <summary>
    /// When invincible is true the character does not take damage.
    /// </summary>
    public bool invincible = false;

    /// <summary>
    /// Intervals of burning damage to take.
    /// </summary>
    private int burning = 0;

    /// <summary>
    /// All powerups that this character currently has.
    /// </summary>
    private List<PowerupRecord> powerups = new List<PowerupRecord>();
    
    AudioSource source;
    Animator anim;
    Stats stats;

    void Awake()
    {
        currentHitpoints = maxHitpoints;
        anim = gameObject.GetComponent<Animator>();
        source = gameObject.GetComponent<AudioSource>();
    }

    void Start()
    {
        stats = GameObject.Find("Stats").GetComponent<Stats>();
        InvokeRepeating("CheckForBurning", 1.0f, 1.0f);
    }

    void Update()
    {
        // Traverse backwards for safe removal while iterating
        for (int i = powerups.Count - 1; i >= 0; --i)
        {
            // Remove expired powerups
            if(powerups[i].pickupTime + powerups[i].powerup.GetDuration() <= Time.time)
            {
                powerups[i].powerup.ApplyEnd(this);
                powerups.RemoveAt(i);
                continue;
            }

            powerups[i].powerup.ApplyUpdate(this);
        }
    }
    
    void FixedUpdate()
    {
        CheckForFallDeath();
    }

    /// <summary>
    /// Sets the targets on fire, dealing burning damage over time.
    /// </summary>
    public void SetOnFire()
    {
        burning = 5;
    }

    /// <summary>
    /// Called on interval to check if we are burning and if so, deal 5 damage per interval
    /// until the burning wears off (does nothing if we're not burning).
    /// </summary>
    private void CheckForBurning()
    {
        if (burning > 0)
        {
            Damage(5);
            --burning;
        }
    }

    /// <summary>
    /// Die if fall off the edge of the level.
    /// </summary>
    private void CheckForFallDeath()
    {
        // Levels are positioned in the first quadrant, so any negative number is off the level. We'll give a little
        // leeway just in case.
        if (transform.position.y < -5)
        {
            currentHitpoints = 0;
            stats.AddFall(gameObject);
        }
    }

    /// <summary>
    /// Inflict some HP damage to this character.
    /// </summary>
    /// <param name="hp">Amount of HP points to deduct.</param>
    public void Damage(int hp)
    {
        if (!invincible) currentHitpoints = currentHitpoints - hp;
        source.Play();
    }

    /// <summary>
    /// Call when this character should be considered dead to reset their HP and reduce their lives.
    /// </summary>
    public void Die()
    {
        currentHitpoints = maxHitpoints;
        --lives;
        Cleanup();
        //anim.SetTrigger("Death");
    }

    /// <summary>
    /// Applies some force on this character as a knockback effect.
    /// </summary>
    /// <param name="forceDirection">The force to apply (direction and magnitude).</param>
    public void DamageForce(Vector3 forceDirection)
    {
        GetComponent<Rigidbody2D>().AddForce(forceDirection);
        anim.SetTrigger("Hit");
    }

    /// <summary>
    /// Performs any cleanup that should occur every time the character dies (eg, removal of projectiles).
    /// </summary>
    protected virtual void Cleanup()
    {
        // Remove all powerups
        for (int i = powerups.Count - 1; i >= 0; --i)
        {
            powerups[i].powerup.ApplyEnd(this);
            powerups.RemoveAt(i);
        }
    }

    public void ConsumePowerup(PowerupBase powerup)
    {
        // Duration zero powerups don't need to be added to the list since they'd just get
        // popped instantly
        powerup.ApplyStart(this);
        if (powerup.GetDuration() != 0f)
        {
            powerups.Add(new PowerupRecord(powerup));
        }
        else
        {
            // Since it's "popped instantly"
            powerup.ApplyEnd(this);
        }
    }
}
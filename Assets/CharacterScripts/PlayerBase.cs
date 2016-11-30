using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Common functionality for human and AI players.
/// </summary>
public class PlayerBase : MonoBehaviour
{
    /// <summary>
    /// Basic amount of horizontal force that is added per second of applied movement. This is basically
    /// acceleration.
    /// </summary>
    public float baseRightMoveForce = 1000f;

    /// <summary>
    /// The vertical force applied per jum.
    /// </summary>
    public float jumpVerticalForce = 400f;

    /// <summary>
    /// Maximum velocity in the horizontal direction. This ensures that once we reach the max
    /// speed, we won't go too fast. Yet we don't have to deal with sluggish acceleration.
    /// </summary>
    float maxHorizontalVelocity = 5f;

    /// <summary>
    /// Time at which we can attack again. Used to add delays for attacks (partially to account for animation
    /// and partially to prevent spamming).
    /// </summary>
    protected float timeCanAttackNext = 0f;

    /// <summary>
    /// True if we're able to jump.
    /// </summary>
    protected bool canJump;

    protected CharacterBase characterBase;
    protected AttackBase attackBase;
    protected Rigidbody2D rigidBody;
    protected Animator animator;

    void Awake()
    {
        characterBase = GetComponent<CharacterBase>();
        attackBase = GetComponent<AttackBase>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Moves the character but caps the horizontal velocity at <see cref="maxHorizontalVelocity"/>.
    /// </summary>
    /// <param name="vector">Movement vector to apply.</param>
    protected void MaximalMove(Vector2 vector)
    {
        // All horizontal movement is modified by the character's soecific movement speed multiplier
        rigidBody.AddForce(new Vector2(vector.x * characterBase.movementSpeedMultiplier, vector.y));

        var horizontalVelocity = rigidBody.velocity.x;
        if (Math.Abs(horizontalVelocity) > maxHorizontalVelocity * characterBase.movementSpeedMultiplier)
        {
            rigidBody.velocity = new Vector2(maxHorizontalVelocity * Math.Sign(horizontalVelocity) * characterBase.movementSpeedMultiplier, rigidBody.velocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Floor")
        {
            canJump = true;
            animator.SetBool("Grounded", true);
        }
    }
}
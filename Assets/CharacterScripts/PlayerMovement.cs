﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Basic amount of horizontal force that is added per second of applied movement. This is basically
    /// acceleration.
    /// </summary>
    public float baseRightMoveForce = 1000f;

    /// <summary>
    /// The vertical force applied per jump (which would only be applied once per tap of the jump key
    /// and when on the ground).
    /// </summary>
    public float jumpForce = 300f;

    /// <summary>
    /// Maximum velocity in the horizontal direction. This ensures that once we reach the max
    /// speed, we won't go too fast. Yet we don't have to deal with sluggish acceleration.
    /// </summary>
    float maxHorizontalVelocity = 5f;

    /// <summary>
    /// Maximum number of jumps between touching the ground (change to allow double jumps).
    /// </summary>
    public int maxJumps = 1;

    /// <summary>
    /// Jumps left until we reach the ground. Reset upon touching anything marked as ground.
    /// </summary>
    public int jumpsLeft;

    /// <summary>
    /// Time at which we can attack again. Used to add delays for attacks (partially to account for animation
    /// and partially to prevent spamming).
    /// </summary>
    private float timeCanAttackNext = 0f;

    /// <summary>
    /// Locks the direction the character can move. such as using an attack means you can't turn around till the attack 
    /// has finished.
    /// </summary>
    private bool directionLocked= false;

    private Rigidbody2D rigidBody;
    private AttackBase attackBase;
    private CharacterBase characterBase;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        attackBase = GetComponent<AttackBase>();
        characterBase = GetComponent<CharacterBase>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        directionLocked = false;
    }
    
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var jump = Input.GetButtonDown("Jump");
        int facingDirection = characterBase.facing;

        if (horizontal > 0 && (!directionLocked || facingDirection==1))
        {
            MaximalMove(new Vector2(baseRightMoveForce, 0) * Time.deltaTime);
            characterBase.facing = 1;
            spriteRenderer.flipX = false;
        }
        else if (horizontal < 0 && (!directionLocked || facingDirection == -1))
        {
            MaximalMove(new Vector2(-baseRightMoveForce, 0) * Time.deltaTime);
            characterBase.facing = -1;
            spriteRenderer.flipX = true;
        }
        if (jump && jumpsLeft > 0)
        {
            --jumpsLeft;

            // Reset vertical velocity before a jump -- prevents wall jumping from being crazy
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

            MaximalMove(new Vector2(0, jumpForce));
            animator.SetBool("Grounded", false);
        }
        animator.SetFloat("Speed", rigidBody.velocity.x);

        if (timeCanAttackNext <= Time.time && Input.GetButtonUp("PrimaryAttack"))
        {
            attackBase.Attack1();
            timeCanAttackNext = Time.time + attackBase.GetAttack1Delay();
        }
        else if (timeCanAttackNext <= Time.time && Input.GetButtonUp("SecondaryAttack"))
        {
            attackBase.Attack2();
            timeCanAttackNext = Time.time + attackBase.GetAttack2Delay();
        }
    }

    /// <summary>
    /// Sets the direction lock
    /// </summary>
    /// <param name="locked">Value to set to Direction Locked. True = locked, False = unlocked</param>
    public void setDirectionLocked(bool locked)
    {
        directionLocked = locked;
    }

    /// <summary>
    /// Moves the character but caps the horizontal velocity at <see cref="maxHorizontalVelocity"/>.
    /// </summary>
    /// <param name="vector">Movement vector to apply.</param>
    private void MaximalMove(Vector2 vector)
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
            jumpsLeft = maxJumps;
            animator.SetBool("Grounded", true);
        }
    }
}
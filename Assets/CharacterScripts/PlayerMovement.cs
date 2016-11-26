using System;
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

    private Rigidbody2D rigidBody;
    private AttackBase attackBase;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        attackBase = GetComponent<AttackBase>();
    }
    
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var jump = Input.GetButtonDown("Jump");
        
        if (horizontal > 0)
        {
            MaximalMove(new Vector2(baseRightMoveForce, 0) * Time.deltaTime);
        }
        else if (horizontal < 0)
        {
            MaximalMove(new Vector2(-baseRightMoveForce, 0) * Time.deltaTime);
        }
        if (jump && jumpsLeft > 0)
        {
            --jumpsLeft;

            // Reset vertical velocity before a jump -- prevents wall jumping from being crazy
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

            MaximalMove(new Vector2(0, jumpForce));
        }
        
        if(timeCanAttackNext <= Time.time && Input.GetButtonUp("PrimaryAttack"))
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
    /// Moves the character but caps the horizontal velocity at <see cref="maxHorizontalVelocity"/>.
    /// </summary>
    /// <param name="vector">Movement vector to apply.</param>
    private void MaximalMove(Vector2 vector)
    {
        rigidBody.AddForce(vector);

        var horizontalVelocity = rigidBody.velocity.x;
        if (Math.Abs(horizontalVelocity) > maxHorizontalVelocity)
        {
            rigidBody.velocity = new Vector2(maxHorizontalVelocity * Math.Sign(horizontalVelocity), rigidBody.velocity.y);
        }
    }
    
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Floor")
        {
            jumpsLeft = maxJumps;
        }
    }
}
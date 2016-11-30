using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Manages the movement of human players (either p0 or p1).
/// </summary>
public class PlayerMovement : PlayerBase
{
    /// <summary>
    /// Maximum number of jumps between touching the ground (change to allow double jumps).
    /// </summary>
    public int maxJumps = 1;

    /// <summary>
    /// Jumps left until we reach the ground. Reset upon touching anything marked as ground.
    /// </summary>
    public int jumpsLeft;
    
    void Update()
    {
        var horizontal = Input.GetAxis(gameObject.name + "_Horizontal");
        var jump = Input.GetButtonDown(gameObject.name + "_Jump");

        if (horizontal > 0 && (!characterBase.directionLocked || characterBase.facing == 1))
        {
            MaximalMove(new Vector2(baseRightMoveForce, 0) * Time.deltaTime);
            if (characterBase.facing < 0) transform.Rotate(0, 180, 0);
            characterBase.facing = 1;
        }
        else if (horizontal < 0 && (!characterBase.directionLocked || characterBase.facing == -1))
        {
            MaximalMove(new Vector2(-baseRightMoveForce, 0) * Time.deltaTime);
            if(characterBase.facing > 0) transform.Rotate(0, 180, 0);
            characterBase.facing = -1;
        }
        if (jump && jumpsLeft > 0)
        {
            --jumpsLeft;

            // Reset vertical velocity before a jump -- prevents wall jumping from being crazy
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

            MaximalMove(new Vector2(0, jumpVerticalForce));
            animator.SetBool("Grounded", false);
        }
        animator.SetFloat("Speed", rigidBody.velocity.x);

        if (timeCanAttackNext <= Time.time && Input.GetButtonUp(gameObject.name + "_PrimaryAttack"))
        {
            attackBase.Attack1();
            timeCanAttackNext = Time.time + attackBase.GetAttack1Delay();
        }
        else if (timeCanAttackNext <= Time.time && Input.GetButtonUp(gameObject.name + "_SecondaryAttack"))
        {
            attackBase.Attack2();
            timeCanAttackNext = Time.time + attackBase.GetAttack2Delay();
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
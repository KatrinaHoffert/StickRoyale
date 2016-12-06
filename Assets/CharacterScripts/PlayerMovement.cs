using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Manages the movement of human players (either p0 or p1).
/// </summary>
public class PlayerMovement : PlayerBase
{
    void Update()
    {
        var horizontal = Input.GetAxis(gameObject.name + "_Horizontal");
        var jump = Input.GetButtonDown(gameObject.name + "_Jump");
        var dropDown = Input.GetButtonDown(gameObject.name + "_Down");

        // Only turn if not direction locked
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

        // Jumping and dropping down mutually exclusive
        if (jump && canJump)
        {
            canJump = false;
            platformGroundedOn = null;

            // Reset vertical velocity before a jump -- prevents wall jumping from being crazy
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

            MaximalMove(new Vector2(0, jumpVerticalForce));
            animator.SetBool("Grounded", false);
            animator.SetTrigger ("Jump");
        }
        // Player must be grounded to do this
        else if(dropDown && canJump)
        {
            var ourPlatform = FindPlatformPlayerIsOn(gameObject);

            // Can only drop down through the kinds of platforms that we can also jump up through. These
            // all have platform effectors.
            if(ourPlatform.GetComponent<PlatformEffector2D>() != null)
            {
                DisablePlatformCollision(ourPlatform.transform, 1f);
            }
        }
        
        animator.SetFloat("Speed", Math.Abs(rigidBody.velocity.x));

        if (timeCanAttackNext <= Time.time && Input.GetButtonUp(gameObject.name + "_PrimaryAttack"))
        {
            attackBase.Attack1();
            animator.SetTrigger("Attack1");
            timeCanAttackNext = Time.time + attackBase.GetAttack1Delay();
        }
        else if (timeCanAttackNext <= Time.time && Input.GetButtonUp(gameObject.name + "_SecondaryAttack"))
        {
            attackBase.Attack2();
            animator.SetTrigger("Attack2");
            timeCanAttackNext = Time.time + attackBase.GetAttack2Delay();
        }
    }

    /// <summary>
    /// Method that moves the player a little bit so that they don't stay on top of another players head.
    /// </summary>
    public void moveOffPlayer()
    {
        MaximalMove(new Vector2(antiStackingHorizontalForce * characterBase.facing, antiStackingVerticalForce));
        Debug.Log("fuck you");
    }

    /// <summary>
    /// Used for detecting if players stack.
    /// </summary>
    /*void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            Debug.Log(coll.gameObject.ToString());
            moveOffPlayer();
        }
    }*/

    /// <summary>
    /// Used for jump resetting.
    /// </summary>
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Floor")
        {
            canJump = true;
            animator.SetBool("Grounded", true);
        }
    }
}
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
        if (jump && canJump)
        {
            canJump = false;

            // Reset vertical velocity before a jump -- prevents wall jumping from being crazy
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

            MaximalMove(new Vector2(0, jumpVerticalForce));
            animator.SetBool("Grounded", false);
			animator.SetTrigger ("Jump");
        }
        animator.SetFloat("Speed", rigidBody.velocity.x);

        if (timeCanAttackNext <= Time.time && Input.GetButtonUp(gameObject.name + "_PrimaryAttack"))
        {
            attackBase.Attack1();
            animator.CrossFade("Attack1",0.05f,-1,2);
            timeCanAttackNext = Time.time + attackBase.GetAttack1Delay();
        }
        else if (timeCanAttackNext <= Time.time && Input.GetButtonUp(gameObject.name + "_SecondaryAttack"))
        {
            attackBase.Attack2();
            animator.SetTrigger("Attack2");
            timeCanAttackNext = Time.time + attackBase.GetAttack2Delay();
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
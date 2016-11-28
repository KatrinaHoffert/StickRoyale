using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Ai : MonoBehaviour
{
    /// <summary>
    /// Basic amount of horizontal force that is added per second of applied movement. This is basically
    /// acceleration.
    /// </summary>
    public float baseRightMoveForce = 1000f;

    /// <summary>
    /// The vertical force applied per jum.
    /// </summary>
    public float jumpVerticalForce = 300f;

    /// <summary>
    /// Maximum velocity in the horizontal direction. This ensures that once we reach the max
    /// speed, we won't go too fast. Yet we don't have to deal with sluggish acceleration.
    /// </summary>
    float maxHorizontalVelocity = 5f;

    /// <summary>
    /// The decision tree that controls AI behavior.
    /// </summary>
    private DecisionTree decisionTree;

    /// <summary>
    /// True when the AI is in the middle of some action (and thus must not try and perform another).
    /// </summary>
    private bool areWeBusy;

    /// <summary>
    /// True if we're able to jump.
    /// </summary>
    private bool canJump;

    private CharacterBase characterBase;
    private AttackBase attackBase;
    private Rigidbody2D rigidBody;
    private Animator animator;

    void Awake()
    {
        characterBase = GetComponent<CharacterBase>();
        attackBase = GetComponent<AttackBase>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        decisionTree = DecisionTree.Decision(
            AreWeBusy,
            ifTrue: DecisionTree.Action(() => {}),
            ifFalse: DecisionTree.Decision(
                AreWeFalling,
                ifTrue: DecisionTree.Action(JumpTowardsFloor),
                ifFalse: DecisionTree.Decision(
                    PlayerInAttackRange,
                    ifTrue: DecisionTree.Decision(
                        AreAttacksOnCooldown,
                        ifTrue: DecisionTree.Action(StepAway),
                        ifFalse: DecisionTree.Action(Attack)
                    ),
                    ifFalse: DecisionTree.Decision(
                        PlayerInAttackRangeIfWeTurn,
                        ifTrue: DecisionTree.Action(Turn),
                        ifFalse: DecisionTree.Action(MoveTowardsPlayer)
                    )
                )
            )
        );
    }

    void FixedUpdate()
    {
        decisionTree.Search();
    }
    
    private bool AreWeBusy()
    {
        return areWeBusy;
    }

    private bool AreWeFalling()
    {
        // If there's a floor beneath us, we're definitely fine. Otherwise we look at our velocity to
        // see if we're falling. This ensures we aren't "falling" if we're falling onto a platform.
        var hits = Physics2D.RaycastAll(transform.position, Vector2.down);
        foreach (var hit in hits)
        {
            if (hit.transform.tag == "Floor") return false;
        }
        return rigidBody.velocity.y < 0;
    }

    private void JumpTowardsFloor()
    {
        // Identify nearest floor object
        var floors = GameObject.FindGameObjectsWithTag("Floor");
        GameObject nearestFloor = floors[0];
        float nearestFloorDistance = Vector2.Distance(transform.position, nearestFloor.transform.position);
        foreach (var floor in floors.Skip(1))
        {
            var distanceToFloor = Vector2.Distance(transform.position, floor.transform.position);
            if (distanceToFloor < nearestFloorDistance)
            {
                // Ignore if directly above
                var platformAngle = Vector2.Angle(transform.position, floor.transform.position);
                if (platformAngle > 75 || platformAngle < 135) continue;

                nearestFloor = floor;
                nearestFloorDistance = distanceToFloor;
            }
        }


        if (canJump)
        {
            // Reset vertical velocity before a jump -- consistent with player
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
            MaximalMove(new Vector2(0, jumpVerticalForce));
            canJump = false;
        }
        
        // Move in direction of platform until on it or under it
        var direction = Math.Sign(nearestFloor.transform.position.x - transform.position.x);
        Debug.Log("Moveing: " + (baseRightMoveForce * direction * Time.fixedDeltaTime));
        MaximalMove(new Vector2(baseRightMoveForce * direction * Time.fixedDeltaTime, 0));
    }

    private bool PlayerInAttackRange()
    {
        // TODO: Implement
        return false;
    }

    private bool AreAttacksOnCooldown()
    {
        // TODO: Implement
        return false;
    }

    private void StepAway()
    {
        // TODO: Implement
    }
    
    private void Attack()
    {
        // TODO: Implement
    }

    private bool PlayerInAttackRangeIfWeTurn()
    {
        // TODO: Implement
        return false;
    }

    private void Turn()
    {
        // TODO: Implement
    }

    private void MoveTowardsPlayer()
    {
        // TODO: Implement
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
            canJump = true;
            animator.SetBool("Grounded", true);
        }
    }
}
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Scripts the entire functionality of the AI. All movement, attacks, everything.
/// </summary>
public class Ai : PlayerBase
{
    /// <summary>
    /// The decision tree that controls AI behavior.
    /// </summary>
    private DecisionTree decisionTree;

    /// <summary>
    /// True when the AI is in the middle of some action (and thus must not try and perform another).
    /// </summary>
    private bool areWeBusy;
    
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
        if(FindPlatformPlayerIsOn(gameObject) != null) return false;
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
        MaximalMove(new Vector2(baseRightMoveForce * direction * Time.fixedDeltaTime, 0));
    }

    private bool PlayerInAttackRange()
    {
        return attackBase.CanAttack1Hit(characterBase.facing) || attackBase.CanAttack2Hit(characterBase.facing);
    }

    private bool AreAttacksOnCooldown()
    {
        return timeCanAttackNext >= Time.time;
    }

    private void StepAway()
    {
        // TODO: Implement
    }
    
    private void Attack()
    {
        // Choose an attack
        if (attackBase.CanAttack1Hit(characterBase.facing) && attackBase.CanAttack2Hit(characterBase.facing))
        {
            // Pick best, randomly weighing
            var attack1Weight = attackBase.GetAttack1AiWeight();
            var attack2Weight = attackBase.GetAttack2AiWeight();
            if (UnityEngine.Random.Range(0, attack1Weight + attack2Weight) <= attack1Weight) Attack1();
            else Attack2();
        }
        else if (attackBase.CanAttack1Hit(characterBase.facing)) Attack1();
        else Attack2();
    }

    private void Attack1()
    {
        attackBase.Attack1();
        animator.SetTrigger("Attack1");
        timeCanAttackNext = Time.time + attackBase.GetAttack1Delay();
    }

    private void Attack2()
    {
        attackBase.Attack2();
        animator.SetTrigger("Attack2");
        timeCanAttackNext = Time.time + attackBase.GetAttack2Delay();
    }

    private bool PlayerInAttackRangeIfWeTurn()
    {
        return attackBase.CanAttack1Hit(characterBase.facing * -1) || attackBase.CanAttack2Hit(characterBase.facing * -1);
    }

    protected void Turn()
    {
        transform.Rotate(0, 180, 0);
        characterBase.facing = characterBase.facing * -1;
    }

    private void MoveTowardsPlayer()
    {
        // First pick a target player -- for now just gonna pick whoever is closest
        var players = GameObject.FindGameObjectsWithTag("Player").Where(player => player != gameObject).ToArray();

        var closestPlayer = players[0];
        var closestPlayerDistance = Vector3.Distance(closestPlayer.transform.position, transform.position);
        for (int i = 1; i < players.Length; ++i)
        {
            var distanceToPlayer = Vector3.Distance(players[i].transform.position, transform.position);
            if (distanceToPlayer < closestPlayerDistance)
            {
                closestPlayer = players[i];
                closestPlayerDistance = distanceToPlayer;
            }
        }

        // Figure out platforms each are on
        var targetPlatform = FindPlatformPlayerIsOn(closestPlayer);
        var ourPlatform = FindPlatformPlayerIsOn(gameObject);

        // Platforms are the same? Just move towards the player.
        if(targetPlatform == ourPlatform)
        {
            // Leave a ~0.5 gap so we don't move with the target on top of us
            var horizontalDistance = closestPlayer.transform.position.x - transform.position.x;
            if(Math.Abs(horizontalDistance) > 0.5)
            {
                // Turn around if there's a change in direction
                int direction = Math.Sign(horizontalDistance);
                if (direction != characterBase.facing) Turn();

                MaximalMove(new Vector2(baseRightMoveForce * direction * Time.fixedDeltaTime, 0));
            }
        }
        // Otherwise figure out how to get to the target's platform -- if they aren't on a platform,
        // do nothing.
        else if(targetPlatform != null)
        {
            // TODO: This might occur if we're hanging at the edge. Should find way to fix that.
            if (ourPlatform == null) return;

            // Find the jump point on our platform in the right direction
            int direction = Math.Sign(closestPlayer.transform.position.x - transform.position.x);
            var jumpSpots = ourPlatform.transform.Cast<Transform>().Where(child => child.tag == "JumpSpot");

            // Take the one in the direction towards our target if there is one or whatever the sole one
            // is, otherwise
            Transform jumpSpot;
            jumpSpot = jumpSpots.Where(spot => Math.Sign(spot.transform.position.x - transform.position.x) == direction).FirstOrDefault();
            if (jumpSpot == null) jumpSpot = jumpSpots.FirstOrDefault();
            
            // If still no jump spots, we're stuck
            if(jumpSpot == null)
            {
                Debug.Log(gameObject.name + " is stuck on " + ourPlatform);
                return;
            }

            // Actually move towards the jump spot
            var distanceToJumpSpot = jumpSpot.position.x - transform.position.x;
            MaximalMove(new Vector2(baseRightMoveForce * direction * Time.fixedDeltaTime, 0));

            // If we're close, jump
            if(Math.Abs(distanceToJumpSpot) < 0.5 && canJump)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
                MaximalMove(new Vector2(300f * direction, jumpVerticalForce));
                canJump = false;
            }
            // Can't jump multiple times, silly. But stop us from moving so we don't lose the spot...
            else if(Math.Abs(distanceToJumpSpot) < 0.5 && !canJump)
            {
                rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
            }

            //Debug.Log(gameObject.name + " moving towards jump spot " + jumpSpot + " (" + distanceToJumpSpot + " away)");
        }

        //Debug.Log(gameObject.name + " is on " + ourPlatform + " and their target (" + closestPlayer.name + ") is on " + targetPlatform);
    }

    /// <summary>
    /// Returns the platform that the player is either on or over (could be in the air above it).
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <returns>The game object for the platform that they're on, if there is one. Null otherwise.</returns>
    private GameObject FindPlatformPlayerIsOn(GameObject player)
    {
        var hits = Physics2D.RaycastAll(player.transform.position, Vector2.down);
        foreach (var hit in hits)
        {
            if (hit.transform.tag == "Floor") return hit.transform.gameObject;
        }
        return null;
    }

    /// <summary>
    /// Method that moves the player a little bit so that they don't stay on top of another players head.
    /// </summary>
    void MoveOffPlayer()
    {
        if (characterBase.facing == 1)
        {
            MaximalMove(new Vector2(50f, -10f));
        }
        else
        {
            MaximalMove(new Vector2(-50f, -10f));
        }
    }
    
    /// <summary>
    /// Checks if player is on top of another Players head, and then calls the function to move them off
    /// of that players head.
    /// </summary>
    /// <param name="coll"></param>
    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            MoveOffPlayer();
        }
    }
}
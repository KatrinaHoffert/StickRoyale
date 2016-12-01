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
    /// A delay that is added between all attacks because the AI is faster than humans. This makes
    /// it a bit slower to give folks a chance.
    /// </summary>
    private const float extraAttackDelay = 0.1f;

    /// <summary>
    /// Distance at which the AI will try and get away from the nearest player when attacks are on
    /// cooldown. See <see cref="StepAway"/>.
    /// </summary>
    private const float stepAwayDistance = 1.5f;

    /// <summary>
    /// Amount of leeway to leave when moving so that when the foe is directly above us, we're not
    /// moving perfectly with them. Most importantly this helps avoid players stacking on top of
    /// others. Especially since we measure the distance from the center of the character and two
    /// characters will NEVER get this close if they are on the same platform.
    /// </summary>
    private const float targetOnTopLeewayDistance = 0.5f;

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
        var closestPlayer = GetClosestPlayer();
        var closestPlayerDistance = closestPlayer.transform.position.x - transform.position.x;

        // Only step away if they are really close
        if (Math.Abs(closestPlayerDistance) < stepAwayDistance)
        {
            // Move in the opposite direction that they are in
            var direction = Math.Sign(closestPlayerDistance) * -1;
            MaximalMove(new Vector2(baseRightMoveForce * direction * Time.fixedDeltaTime, 0));
        }
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
        timeCanAttackNext = Time.time + attackBase.GetAttack1Delay() + extraAttackDelay;
    }

    private void Attack2()
    {
        attackBase.Attack2();
        animator.SetTrigger("Attack2");
        timeCanAttackNext = Time.time + attackBase.GetAttack2Delay() + extraAttackDelay;
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
        var closestPlayer = GetClosestPlayer();

        // Figure out platforms each are on
        var targetPlatform = FindPlatformPlayerIsOn(closestPlayer);
        var ourPlatform = FindPlatformPlayerIsOn(gameObject);

        // Platforms are the same? Just move towards the player.
        if(targetPlatform == ourPlatform)
        {
            // Leave a gap so we don't move with the target on top of us
            var horizontalDistance = closestPlayer.transform.position.x - transform.position.x;
            if(Math.Abs(horizontalDistance) > targetOnTopLeewayDistance)
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
    /// Gets the player closest to this AI character (absolute distance, not necessarily the easiest to attack).
    /// </summary>
    /// <returns>The closest player.</returns>
    private GameObject GetClosestPlayer()
    {
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
        return closestPlayer;
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

        // Maybe the raycast from the center is to blame? Try raycasts from sides too, but only if
        // there's no y velocity (could be falling).
        if(player.GetComponent<Rigidbody2D>().velocity.y == 0)
        {
            var hitsLeft = Physics2D.RaycastAll(player.transform.position - new Vector3(0.5f, 0), Vector2.down);
            var hitsRight = Physics2D.RaycastAll(player.transform.position + new Vector3(0.5f, 0), Vector2.down);

            var floorHitLeft = hitsLeft.Where(hit => hit.transform.tag == "Floor").FirstOrDefault();
            var floorHitRight = hitsLeft.Where(hit => hit.transform.tag == "Floor").FirstOrDefault();
            
            // If we only get one hit, return that. Otherwise the closest
            if (floorHitLeft.transform == null && floorHitRight.transform != null)
            {
                return floorHitRight.transform.gameObject;
            }
            else if (floorHitLeft.transform != null && floorHitRight.transform == null)
            {
                return floorHitLeft.transform.gameObject;
            }
            else if (floorHitLeft.transform != null && floorHitRight.transform != null)
            {
                return floorHitLeft.distance < floorHitRight.distance ? floorHitLeft.transform.gameObject : floorHitRight.transform.gameObject;
            }
        }

        return null;
    }

    /// <summary>
    /// Method that moves the player a little bit so that they don't stay on top of another players head.
    /// </summary>
    void MoveOffPlayer()
    {
        MaximalMove(new Vector2(antiStackingHorizontalForce * characterBase.facing, antiStackingVerticalForce));
    }
    
    /// <summary>
    /// Used for detecting player stacking.
    /// </summary>
    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            MoveOffPlayer();
        }
    }
}
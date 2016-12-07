using System;
using System.Collections;
using System.Collections.Generic;
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
    private const float stepAwayDistance = 3f;

    /// <summary>
    /// Amount of leeway to leave when moving so that when the foe is directly above us, we're not
    /// moving perfectly with them. Most importantly this helps avoid players stacking on top of
    /// others. Especially since we measure the distance from the center of the character and two
    /// characters will NEVER get this close if they are on the same platform.
    /// </summary>
    private const float targetOnTopLeewayDistance = 0.5f;

    /// <summary>
    /// When moving towards a player, we need to save the jump spot to use, which only applies for a
    /// pathfinding attempt from our platform to a target platform. Hence, we must save those too.
    /// </summary>
    private GameObject targetPlatformForMovement;

    /// <summary>
    /// <see cref="targetPlatformForMovement"/>.
    /// </summary>
    private GameObject ourPlatformForMovement;

    /// <summary>
    /// The jump spot that the AI is moving towards iff <see cref="targetPlatformForMovement"/> and
    /// <see cref="ourPlatformForMovement"/> are correct.
    /// </summary>
    private GameObject jumpSpotToUse;

    /// <summary>
    /// The powerup that we are seeking, if there is one. If <see cref="ShouldWeSeekOutAPowerup"/>
    /// returns true, then this must be non-null.
    /// </summary>
    private GameObject powerupWeAreSeeking;

    /// <summary>
    /// To add some randomness, the AI has a chance not to pursue any powerup at all per frame.
    /// This thus multiplies with the chance to actually pursue the powerup based on desire
    /// for it.
    /// </summary>
    private float chanceToPursuePowerupPerFrame = 0.05f;

    /// <summary>
    /// True when the AI is in the middle of continuing movement from a jump.
    /// </summary>
    private bool continuingJumpMovement;

    /// <summary>
    /// Chance *per frame* of deciding to dodge an incoming mage's projectile.
    /// </summary>
    private float dodgeChance = 0.1f;

    /// <summary>
    /// The rate at which we reduce our velocity per frame when we have jumped and are now
    /// over a different platfrom. Should be in [0, 1]. Larger values mean less slowdown (more
    /// likely to overshoot jumps). Smaller values look unnatural and are kinda cheaty.
    /// </summary>
    private float fallingOntoPlatformSlowdown = 0.9f;

    void Start()
    {
        decisionTree = DecisionTree.Decision(
            AreWeContinuingMovement,
            ifTrue: DecisionTree.Action(ContinueJumpMovement),
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
                        ifFalse: DecisionTree.Decision(
                            ShouldWeDodgeProjectile,
                            ifTrue: DecisionTree.Action(DodgeProjectile),
                            ifFalse: DecisionTree.Decision(
                                ShouldWeSeekOutAPowerup,
                                ifTrue: DecisionTree.Action(MoveTowardsPowerup),
                                ifFalse: DecisionTree.Action(MoveTowardsPlayer)
                            )
                        )
                    )
                )
            )
        );
    }

    void Update()
    {
        decisionTree.Search();
    }

    private bool AreWeContinuingMovement()
    {
        return continuingJumpMovement && !canJump;
    }

    private void ContinueJumpMovement()
    {
        // Continue movement in whatever direction we're already going in unless we're over a different platform.
        var ourCurrentPlatform = FindPlatformPlayerIsOn(gameObject);
        if (ourCurrentPlatform == ourPlatformForMovement)
        {
            MaximalMove(new Vector2(baseRightMoveForce * characterBase.facing * Time.deltaTime, 0));
        }
        else if (ourCurrentPlatform != null)
        {
            // Slow us gradually so we can fall onto that platform
            rigidBody.velocity = new Vector2(rigidBody.velocity.x * fallingOntoPlatformSlowdown, rigidBody.velocity.y);
        }
    }

    private bool AreWeFalling()
    {
        if (FindPlatformPlayerIsOn(gameObject) != null) return false;
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
            platformGroundedOn = null;
        }

        // Move in direction of platform until on it or under it
        var direction = Math.Sign(nearestFloor.transform.position.x - transform.position.x);
        MaximalMove(new Vector2(baseRightMoveForce * direction * Time.deltaTime, 0));
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
            rigidBody.velocity = new Vector2(rigidBody.velocity.x / 2, rigidBody.velocity.y);
            MaximalMove(new Vector2(baseRightMoveForce * direction * Time.deltaTime, 0));
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

    private bool ShouldWeDodgeProjectile()
    {
        foreach (var projectile in FindObjectsOfType<MageAttack2Trigger>())
        {
            // Don't dodge if we were close to the caster -- that's cheating!
            if (Math.Abs(projectile.casterObject.transform.position.x - transform.position.x) < 4) return false;

            if (Math.Abs(projectile.transform.position.y - transform.position.y) < 2
                && Math.Abs(projectile.transform.position.x - transform.position.x) < 2
                && UnityEngine.Random.Range(0, 1) < dodgeChance)
            {
                return true;
            }
        }

        return false;
    }

    private void DodgeProjectile()
    {
        if (canJump)
        {
            canJump = false;
            MaximalMove(new Vector2(0, jumpVerticalForce));
            animator.SetBool("Grounded", false);
            animator.SetTrigger("Jump");
        }
    }

    private bool ShouldWeSeekOutAPowerup()
    {
        var powerupsAvailable = FindObjectsOfType<PowerupBase>();

        // Trivial case: no powerups exist
        if (powerupsAvailable.Count() == 0) return false;

        // If we've already decided we're seeking out a powerup and it's still there,
        // keep at it, little guy. TODO: Re-evaluate if this powerup is still worth
        // going for.
        if (powerupWeAreSeeking != null) return true;

        // Should we even consider powerups right now?
        var randomWeighting = UnityEngine.Random.value;
        if (randomWeighting > chanceToPursuePowerupPerFrame) return false;

        // If another character is less than half of our distance from the powerup, don't bother.
        var otherPlayers = FindObjectsOfType<CharacterBase>().Where(c => c.gameObject != gameObject);
        var random = UnityEngine.Random.value;
        foreach (var powerup in powerupsAvailable)
        {
            var ourDistance = Vector3.Distance(transform.position, powerup.transform.position);
            foreach(var player in otherPlayers)
            {
                var distanceToThatPlayer = Vector3.Distance(player.transform.position, powerup.transform.position);
                if (distanceToThatPlayer < ourDistance / 2) continue;
            }

            // Powerup is viable, see if we'd pick it
            if(random < powerup.GetAiWeight(characterBase))
            {
                powerupWeAreSeeking = powerup.gameObject;
                return true;
            }
        }

        // Didn't choose any powerups
        return false;
    }

    private void MoveTowardsPowerup()
    {
        MoveTowardsTarget(powerupWeAreSeeking);
    }

    private void MoveTowardsPlayer()
    {
        // First pick a target player -- for now just gonna pick whoever is closest
        var closestPlayer = GetClosestPlayer();
        MoveTowardsTarget(closestPlayer);
    }

    /// <summary>
    /// The process of moving the AI towards some target object. The process can be summed up as:
    /// 
    /// 1. If we're on the same platform, just move towards them, leaving some leeway in case they're
    ///    in the air.
    /// 2. If we're on different platforms and not on a platform above them, we identify the pre-placed
    ///    jump spots on our platform. If there's just one, we obviously have to take it. Otherwise
    ///    we pick the one in the direction of our target.
    /// 3. We move towards the jump spot. Once we're close to it, we jump in the direction towards the
    ///    target.
    /// 4. If the player is on a platform below, we can drop down through the platform instead.
    /// </summary>
    /// <param name="target">The target to seek out.</param>
    private void MoveTowardsTarget(GameObject target)
    {
        continuingJumpMovement = false;

        // Figure out platforms each are on
        var targetPlatform = FindPlatformPlayerIsOn(target);
        var ourPlatform = FindPlatformPlayerIsOn(gameObject);

        // Can't do anything if we don't know platforms
        if (targetPlatform == null || ourPlatform == null) return;

        // Platforms are the same? Just move towards the player.
        if (targetPlatform == ourPlatform)
        {
            // Leave a gap so we don't move with the target on top of us
            var horizontalDistance = target.transform.position.x - transform.position.x;
            if (Math.Abs(horizontalDistance) > targetOnTopLeewayDistance)
            {
                // Turn around if there's a change in direction
                int direction = Math.Sign(horizontalDistance);
                if (direction != characterBase.facing) Turn();

                MaximalMove(new Vector2(baseRightMoveForce * direction * Time.deltaTime, 0));
            }
        }
        // Otherwise figure out how to get to the target's platform -- if they aren't on a platform,
        // do nothing.
        else if (targetPlatform != null)
        {
            // Handle dropping down from platforms if we're above the target
            if (ourPlatform.transform.position.y > targetPlatform.transform.position.y)
            {
                var rayHits = Physics2D.RaycastAll(transform.position, Vector2.down, 100f);
                if (Math.Abs(Math.Abs(transform.position.x) - Math.Abs(target.transform.position.x)) < 1 ||
                    rayHits.Where(hit => hit.transform.gameObject == targetPlatform).Count() > 0)
                {
                    // To avoid falling through a platform and velocity carrying us off the edge, reduce that
                    rigidBody.velocity = new Vector2(rigidBody.velocity.x * 0.25f, rigidBody.velocity.y);

                    DisablePlatformCollision(ourPlatform.transform, 2f);
                    return;
                }
            }

            // Move towards the jump spot
            var jumpSpot = ChooseJumpSpot(ourPlatform, targetPlatform, target);
            int directionToJumpSpot = Math.Sign(jumpSpot.position.x - transform.position.x);
            var distanceToJumpSpot = jumpSpot.position.x - transform.position.x;
            MaximalMove(new Vector2(baseRightMoveForce * directionToJumpSpot * Time.deltaTime, 0));
            if (directionToJumpSpot != characterBase.facing) Turn();

            // If we're close, jump
            if (Math.Abs(distanceToJumpSpot) < 0.5 && canJump)
            {
                int directionToTarget = Math.Sign(target.transform.position.x - transform.position.x);

                // See if the jump spot has a hard coded direction we should be jumping to get to our
                // target (otherwise stick with default behavior)
                var jumpSpotDirectionList = jumpSpot.GetComponent<JumpSpot>().jumpDirections;
                var directionsToTarget = jumpSpotDirectionList.Where(directions => directions.targetPlatform == targetPlatform).FirstOrDefault();
                if (directionsToTarget != null) directionToTarget = directionsToTarget.direction;

                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
                MaximalMove(new Vector2(300f * directionToTarget, jumpVerticalForce));
                canJump = false;
                platformGroundedOn = null;
                if (directionToTarget != 0) continuingJumpMovement = true;

                // Make sure we face the right way
                if (directionToTarget != characterBase.facing) Turn();
            }

            //Debug.Log(gameObject.name + " moving towards jump spot " + jumpSpot + " (" + distanceToJumpSpot + " away)");
        }

        //Debug.Log(gameObject.name + " is on " + ourPlatform + " and their target (" + target.name + ") is on " + targetPlatform);
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
    /// Selects the jump spot that we're gonna move towards. Assumes we are on a different platform than
    /// the target.
    /// </summary>
    /// <param name="ourPlatform">Platform we are on.</param>
    /// <param name="targetPlatform">Platform target is on.</param>
    /// <param name="target">Target we are seeking out.</param>
    /// <returns>Transform of the jump spot to move towards.</returns>
    private Transform ChooseJumpSpot(GameObject ourPlatform, GameObject targetPlatform, GameObject target)
    {
        // Do we already have a jump spot we should be moving towards?
        Transform jumpSpot = null;
        if (targetPlatformForMovement == targetPlatform && ourPlatformForMovement == ourPlatform)
        {
            jumpSpot = jumpSpotToUse.transform;
        }
        else
        {
            // Find the jump point on our platform in the right direction
            int directionToTarget = Math.Sign(target.transform.position.x - transform.position.x);
            var jumpSpots = ourPlatform.transform.Cast<Transform>().Where(child => child.tag == "JumpSpot");

            // Take the closest in the direction of our target or that can reach all
            var distanceToChosenSpot = float.PositiveInfinity;
            foreach (var possibleJumpSpot in jumpSpots)
            {
                var distanceToPossibleJumpSpot = Math.Abs(possibleJumpSpot.transform.position.x - transform.position.x);
                var directionPossibleToJumpSpot = Math.Sign(possibleJumpSpot.transform.position.x - transform.position.x);
                if (directionPossibleToJumpSpot == directionToTarget || possibleJumpSpot.GetComponent<JumpSpot>().canReachAllPlatforms)
                {
                    if (distanceToPossibleJumpSpot < distanceToChosenSpot)
                    {
                        distanceToChosenSpot = distanceToPossibleJumpSpot;
                        jumpSpot = possibleJumpSpot.transform;
                    }
                }
            }

            // Haven't found one? Get a different one.
            if (jumpSpot == null)
                jumpSpot = jumpSpots.FirstOrDefault();

            // Cache the targets for next time
            ourPlatformForMovement = ourPlatform;
            targetPlatformForMovement = targetPlatform;
            jumpSpotToUse = jumpSpot.gameObject;
        }

        // If still no jump spots, we're stuck
        if (jumpSpot == null)
        {
            Debug.Log(gameObject.name + " is stuck on " + ourPlatform);
            ourPlatformForMovement = null;
            targetPlatformForMovement = null;
            return null;
        }

        return jumpSpot;
    }

    /// <summary>
    /// Method that moves the player a little bit so that they don't stay on top of another players head.
    /// </summary>
    public void moveOffPlayer()
    {
        MaximalMove(new Vector2(antiStackingHorizontalForce * characterBase.facing, antiStackingVerticalForce));
    }
}
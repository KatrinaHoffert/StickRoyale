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
    /// Force applied when a player stacks on top of another, to prevent them from staying on top.
    /// </summary>
    protected const float antiStackingHorizontalForce = 50f;

    /// <summary>
    /// Downward force for when players are stacked to ensure that they will be unstacked.
    /// </summary>
    protected const float antiStackingVerticalForce = -10f;

    /// <summary>
    /// True if we're able to jump.
    /// </summary>
    protected bool canJump;

    /// <summary>
    /// The platform that we are on if not in the air.
    /// </summary>
    protected GameObject platformGroundedOn = null;

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
    
    /// <summary>
    /// Returns the platform that the player is either on or over (could be in the air above it).
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <returns>The game object for the platform that they're on, if there is one. Null otherwise.</returns>
    protected GameObject FindPlatformPlayerIsOn(GameObject player)
    {
        var playerBase = player.GetComponent<PlayerBase>();
        if (playerBase.platformGroundedOn != null) return playerBase.platformGroundedOn;

        var hits = Physics2D.RaycastAll(player.transform.position, Vector2.down);
        foreach (var hit in hits)
        {
            if (hit.transform.tag == "Floor") return hit.transform.gameObject;
        }

        // Maybe the raycast from the center is to blame? Try raycasts from sides too, but only if
        // there's no y velocity (could be falling).
        if (player.GetComponent<Rigidbody2D>().velocity.y == 0)
        {
            var bounds = GetComponent<BoxCollider2D>().bounds;
            var distanceFuzz = 0.05f;
            var hitsLeft = Physics2D.RaycastAll(bounds.center - new Vector3(bounds.extents.x + distanceFuzz, 0), Vector2.down);
            var hitsRight = Physics2D.RaycastAll(bounds.center + new Vector3(bounds.extents.x + distanceFuzz, 0), Vector2.down);
            //Debug.DrawRay(bounds.center - new Vector3(bounds.extents.x + distanceFuzz, 0), Vector2.down * 100, Color.red, 1.0f);
            //Debug.DrawRay(bounds.center + new Vector3(bounds.extents.x + distanceFuzz, 0), Vector2.down * 100, Color.red, 1.0f);

            var floorHitLeft = hitsLeft.Where(hit => hit.transform.tag == "Floor").FirstOrDefault();
            var floorHitRight = hitsRight.Where(hit => hit.transform.tag == "Floor").FirstOrDefault();

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
    /// Disables collision with a given platform. Used so that we can fall through platforms.
    /// </summary>
    /// <param name="platform">The platform in question.</param>
    /// <param name="duration">How long to disable collision for.</param>
    protected void DisablePlatformCollision(Transform platform, float duration)
    {
        Physics2D.IgnoreCollision(transform.GetComponent<Collider2D>(), platform.GetComponent<Collider2D>(), true);
        StartCoroutine(ReenablePlatformCollision(platform, duration));
    }

    /// <summary>
    /// Re-enables collision between this character and a given platform.
    /// </summary>
    /// <param name="platform">The transform of the platform in question.</param>
    /// <param name="delay">Delay to wait before enabling collision again.</param>
    /// <returns></returns>
    protected IEnumerator ReenablePlatformCollision(Transform platform, float delay)
    {
        yield return new WaitForSeconds(delay);
        Physics2D.IgnoreCollision(transform.GetComponent<Collider2D>(), platform.GetComponent<Collider2D>(), false);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Floor")
        {
            canJump = true;
            platformGroundedOn = coll.gameObject;
            animator.SetBool("Grounded", true);
        }
    }
}
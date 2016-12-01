using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// A variety of features that are common to every character. Managing of HP and lives are the
/// most important here, but there's also some movement related stuff.
/// </summary>
public class CharacterBase : MonoBehaviour
{
    /// <summary>
    /// Max HP. When it hits zero, the character dies.
    /// </summary>
    public int maxHitpoints = 100;

    /// <summary>
    /// Current HP.
    /// </summary>
    public int currentHitpoints;

    /// <summary>
    /// Number of lives this player has.
    /// </summary>
    public int lives = 3;

    /// <summary>
    /// The direction that the character is facing. 1 for right, -1 for left.
    /// </summary>
    public int facing = 1;

    /// <summary>
    /// A multiplier to movement speed that this character has.
    /// </summary>
    public float movementSpeedMultiplier = 1.0f;

    /// <summary>
    /// Locks the direction the character can move. such as using an attack means you can't turn around till the attack 
    /// has finished.
    /// </summary>
    public bool directionLocked = false;

    Animator anim;

    void Start()
    {
        currentHitpoints = maxHitpoints;
        anim = gameObject.GetComponent<Animator>();
    }
    
    void FixedUpdate()
    {
        CheckForFallDeath();
    }

    /// <summary>
    /// Die if fall off the edge of the level.
    /// </summary>
    private void CheckForFallDeath()
    {
        // Levels are positioned in the first quadrant, so any negative number is off the level. We'll give a little
        // leeway just in case.
        if (transform.position.y < -5)
        {
            currentHitpoints = 0;
        }
    }

    /// <summary>
    /// Inflict some HP damage to this character.
    /// </summary>
    /// <param name="hp">Amount of HP points to deduct.</param>
    public void Damage(int hp)
    {
        currentHitpoints = currentHitpoints - hp;
        
    }

    /// <summary>
    /// Call when this character should be considered dead to reset their HP and reduce their lives.
    /// </summary>
    public void Die()
    {
        currentHitpoints = maxHitpoints;
        --lives;
        //anim.SetTrigger("Death");
    }

    /// <summary>
    /// Applies some force on this character as a knockback effect.
    /// </summary>
    /// <param name="forceDirection">The force to apply (direction and magnitude).</param>
    public void DamageForce(Vector3 forceDirection)
    {
        GetComponent<Rigidbody2D>().AddForce(forceDirection);
		anim.SetTrigger("Hit");
    }
}
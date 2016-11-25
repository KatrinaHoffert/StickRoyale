using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    void Start()
    {
        currentHitpoints = maxHitpoints;
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

    public void Die()
    {
        currentHitpoints = maxHitpoints;
        --lives;
    }
    
    /// <summary>
    /// when a player collides with this player take 1 damage
    /// </summary>
    /// <param name="col">The object we've collided with.</param>
    void OnCollisionEnter2D(Collision2D col)
    {
        // TODO: This is a dummy method to test damage taking
        if(col.gameObject.GetComponent<CharacterBase>() != null)
        {
            Damage(1);
        }
    }
}
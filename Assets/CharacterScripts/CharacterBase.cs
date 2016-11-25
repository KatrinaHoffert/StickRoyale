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

    void Start()
    {
        currentHitpoints = maxHitpoints;
    }
    
    void FixedUpdate()
    {
        CheckForFallDeath();
    }

    /// <summary>
    /// Die if below certain y coordinate.
    /// </summary>
    void CheckForFallDeath()
    {
        if (transform.position.y < -300)
        {
            currentHitpoints = 0;
        }
    }

    /// <summary>
    /// Inflict some HP damage to this character.
    /// </summary>
    /// <param name="hp">Amount of HP points to deduct.</param>
    void Damage(int hp)
    {
        currentHitpoints = currentHitpoints - hp;
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
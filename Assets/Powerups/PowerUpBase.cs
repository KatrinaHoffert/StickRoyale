using UnityEngine;
using System.Collections;

/// <summary>
/// Basic functionality of the powerups. Contains all the details needed to grab and use this powerup. The
/// powerup must also be added to the <see cref="PowerupManager"/>.
/// </summary>
public abstract class PowerupBase : MonoBehaviour
{
    /// <summary>
    /// Gets the duration that the powerup is supposed to last, in seconds. Zero is a special
    /// case as it will never be added to the character's powerups list and the <see cref="ApplyEnd"/>
    /// method will be called right after <see cref="ApplyStart"/>. It will never see
    /// <see cref="ApplyUpdate"/> tick.
    /// </summary>
    public abstract float GetDuration();

    /// <summary>
    /// A weighting between 0 and 1 for how much a given AI should care about getting this powerup.
    /// </summary>
    /// <param name="character">The AI character in question.</param>
    /// <returns>A weight for AI decision making where 1 is a higher desire to get the powerup and
    /// 0 is a lower desire.</returns>
    public abstract float GetAiWeight(CharacterBase character);

    /// <summary>
    /// Called once when the powerup is grabbed.
    /// </summary>
    /// <param name="character">The character to apply to.</param>
    public virtual void ApplyStart(CharacterBase character) { }
    
    /// <summary>
    /// Called when the powerup expires. Should be used to undo temporary effects.
    /// </summary>
    /// <param name="character">The character to apply to.</param>
    public virtual void ApplyEnd(CharacterBase character) { }

    /// <summary>
    /// Called every frame while the power up is active. Use to apply continuous or changing effects.
    /// </summary>
    /// <param name="character">The character to apply to.</param>
    public virtual void ApplyUpdate(CharacterBase character) { }
    
    /// <summary>
    /// Handles the grabbing of the powerup.
    /// </summary>
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            var founderCharacterBase = coll.gameObject.GetComponent<CharacterBase>();
            founderCharacterBase.ConsumePowerup(this);

            Destroy(gameObject);
        }
    }
}

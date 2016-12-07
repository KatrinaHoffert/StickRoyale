using UnityEngine;
using System.Collections;

public class PowerupFire : PowerupBase
{
    /// <summary>
    /// Time that this powerup was obtained, and hence when the animation looping is based from.
    /// </summary>
    public float animationStartTime;
    
    /// <summary>
    /// Transition period in seconds.
    /// </summary>
    float period = 1f;

    /// <summary>
    /// First color to transition between.
    /// </summary>
    public Color color1 = Color.red;

    /// <summary>
    /// Second colour to transition between.
    /// </summary>
    public Color color2 = Color.yellow;

    public override float GetDuration()
    {
        return 10f;
    }

    public override float GetAiWeight(CharacterBase character)
    {
        // It's not a priority if we already have a damage multiplier
        if (character.onFire) return 0.1f;
        else return 1.0f;
    }

    public override void ApplyStart(CharacterBase character)
    {
        character.onFire = true;
        animationStartTime = Time.time;
    }

    public override void ApplyUpdate(CharacterBase character)
    {
        var elapsedTime = Time.time - animationStartTime;
        var progressBetweenColours = (elapsedTime % period) / period;
        var deltaR = color2.r - color1.r;
        var deltaG = color2.g - color1.g;
        var deltaB = color2.b - color1.b;
        character.gameObject.GetComponent<SpriteRenderer>().color = new Color(
            color1.r + deltaR * progressBetweenColours,
            color1.b + deltaG * progressBetweenColours,
            color1.g + deltaB * progressBetweenColours
        );
    }

    public override void ApplyEnd(CharacterBase character)
    {
        character.onFire = false;
        character.GetComponent<SpriteRenderer>().color = Color.white;
    }
}

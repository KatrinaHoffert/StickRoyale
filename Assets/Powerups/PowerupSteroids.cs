using UnityEngine;
using System.Collections;

public class PowerupSteroids : PowerupBase
{
    public override float GetDuration()
    {
        return 5f;
    }

    public override float GetAiWeight(CharacterBase character)
    {
        // It's not a priority if we already have a damage multiplier
        if (character.damageMultiplier > 1) return 0.1f;
        else return 1.0f;
    }

    public override void ApplyStart(CharacterBase character)
    {
        character.damageMultiplier = 1.25f;
        character.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public override void ApplyEnd(CharacterBase character)
    {
        character.damageMultiplier = 1f;
        character.GetComponent<SpriteRenderer>().color = Color.white;
    }
}

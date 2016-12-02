using UnityEngine;
using System.Collections;

public class PowerupBurger : PowerupBase
{
    public override float GetDuration()
    {
        return 0f;
    }

    public override float GetAiWeight(CharacterBase character)
    {
        return 1.0f - character.currentHitpoints / (float)character.maxHitpoints;
    }

    public override void ApplyStart(CharacterBase character)
    {
        character.currentHitpoints += 25;
        if (character.currentHitpoints > character.maxHitpoints) character.currentHitpoints = character.maxHitpoints;
    }
}

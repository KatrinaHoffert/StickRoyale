using UnityEngine;
using System.Collections;

public class PowerupFire : PowerupBase
{
    public override float GetDuration()
    {
        return 20f;
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
        character.GetComponent<SpriteRenderer>().color = Color.black;
    }

    public override void ApplyEnd(CharacterBase character)
    {
        character.onFire = false;
        character.GetComponent<SpriteRenderer>().color = Color.white;
    }
}

using UnityEngine;
using System.Collections;

public class PowerupInvincible : PowerupBase
{
    public override float GetDuration()
    {
        return 5f;
    }

    public override float GetAiWeight(CharacterBase character)
    {
        
        return 2.0f;
    }

    public override void ApplyStart(CharacterBase character)
    {
        character.invincible = true;
        character.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public override void ApplyEnd(CharacterBase character)
    {
        character.invincible = false;
        character.GetComponent<SpriteRenderer>().color = Color.white;
    }
}

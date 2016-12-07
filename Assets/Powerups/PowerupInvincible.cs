using UnityEngine;
using System.Collections;
using System;

public class PowerupInvincible : PowerupBase
{
    /// <summary>
    /// Period of blinking phases.
    /// </summary>
    public float blinkPeriod = 0.2f;

    public float animationStartTime;

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
        animationStartTime = Time.time;
    }

    public override void ApplyUpdate(CharacterBase character)
    {
        var elapsedTime = Time.time - animationStartTime;
        character.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, (elapsedTime % blinkPeriod) / blinkPeriod);
    }

    public override void ApplyEnd(CharacterBase character)
    {
        character.invincible = false;
        character.GetComponent<SpriteRenderer>().color = Color.white;
    }
}

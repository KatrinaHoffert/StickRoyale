using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MageAttack1Trigger : AttackTriggerBase
{
    /// <summary>
    /// The game object of the caster, who we don't want getting hurt by their own attack.
    /// </summary>
    public GameObject casterObject;

    protected override int GetDamage()
    {
        return 35;
    }

    protected override Vector2 GetDamageForce()
    {
        return new Vector2(0.25f, 1.0f) * 100;
    }

    protected override CharacterBase GetAttackerCharacterBase()
    {
        return casterObject.GetComponent<CharacterBase>();
    }

    protected override int GetAttackDirection(CharacterBase attackerCharacter)
    {
        return GetComponent<SpriteRenderer>().flipX ? -1 : 1;
    }
}

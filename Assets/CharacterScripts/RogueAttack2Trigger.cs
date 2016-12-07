using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RogueAttack2Trigger : AttackTriggerBase
{
    protected override int GetDamage()
    {
        return 25;
    }

    protected override Vector2 GetDamageForce()
    {
        return new Vector2(1.0f, 0.75f) * 150;
    }
}

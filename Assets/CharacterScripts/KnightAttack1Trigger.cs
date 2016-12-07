using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class KnightAttack1Trigger : AttackTriggerBase
{
    protected override int GetDamage()
    {
        return 24;
    }

    protected override Vector2 GetDamageForce()
    {
        return Vector3.right * 50;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MageAttacks : AttackBase
{
    public GameObject mageAttack1Prefab;

    private Queue<GameObject> attack1Projectiles = new Queue<GameObject>();

    public override void Attack1()
    {
        // The projectile has been rotated so that it looks like it's coming out of the ground.
        // Note: This also affects the direction of force being applied by the collision.
        var attackObject = (GameObject) Instantiate(mageAttack1Prefab, transform.position + new Vector3(characterBase.facing * 0.5f, -0.25f, 0),
                Quaternion.AngleAxis(90, Vector3.forward * characterBase.facing));
        attack1Projectiles.Enqueue(attackObject);
        attackObject.GetComponent<MageAttack1Trigger>().casterObject = gameObject;

        // Face the projectile the right way
        if (characterBase.facing < 0) attackObject.GetComponent<SpriteRenderer>().flipX = true;

        // Remove the projectile later
        Invoke("ClearAttack1Projectile", 0.5f);
    }

    /// <summary>
    /// Clears the oldest object that is created by Attack1
    /// </summary>
    private void ClearAttack1Projectile()
    {
        Destroy(attack1Projectiles.Dequeue());
    }

    public override void Attack2()
    {
        // TODO
    }

    public override float GetAttack1Delay()
    {
        return 0.5f;
    }

    public override float GetAttack2Delay()
    {
        return 0.25f;
    }

    public override bool CanAttack1Hit()
    {
        // TODO: Placeholder
        return false;
    }

    public override bool CanAttack2Hit()
    {
        // TODO: Placeholder
        return false;
    }
    
    public override double GetAttack1AiWeight()
    {
        // TODO: Placeholder
        return 1.0;
    }

    public override double GetAttack2AiWeight()
    {
        // TODO: Placeholder
        return 1.0;
    }
}

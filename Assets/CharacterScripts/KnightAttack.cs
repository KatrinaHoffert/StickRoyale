using UnityEngine;
using System.Collections;

public class KnightAttack : AttackBase
{
    private GameObject attack1Prefab;
    private GameObject attack2Prefab;
    
    void Start()
    {
        // Assign the colliders in the attack prefabs so they can be called upon
        Transform[] childTransforms = GetComponentsInChildren<Transform>();
        foreach (Transform transform in childTransforms)
        {
            if (transform.tag == "Attack1")
            {
                attack1Prefab = transform.gameObject;
            }
            if (transform.tag == "Attack2")
            {
                attack2Prefab = transform.gameObject;
            }
        }
    }
    
    public override void Attack1()
    {
        attack1Prefab.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<PlayerMovement>().setDirectionLocked(true);
        Invoke("resetAttack", GetAttack1Delay());
    }
    
    public override void Attack2()
    {
        attack2Prefab.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<PlayerMovement>().setDirectionLocked(true);
        Invoke("resetAttack", GetAttack2Delay());
    }
    
    public void resetAttack()
    {
        attack1Prefab.GetComponent<BoxCollider2D>().enabled = false;
        attack2Prefab.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<PlayerMovement>().setDirectionLocked(false);
    }

    public override float GetAttack1Delay()
    {
        return 0.5f;
    }

    public override float GetAttack2Delay()
    {
        return 0.5f;
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

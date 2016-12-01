using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RogueAttacks : AttackBase
{
    private GameObject attack1Prefab;
    private GameObject attack2Prefab;

    void Awake()
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

        // Make sure that the attack AoEs aren't initially active
        attack1Prefab.GetComponent<BoxCollider2D>().enabled = false;
        attack2Prefab.GetComponent<BoxCollider2D>().enabled = false;
    }

    public override void Attack1()
    {
        attack1Prefab.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<CharacterBase>().directionLocked = true;
        Invoke("resetAttack", GetAttack1Delay());
    }

    public override void Attack2()
    {
        attack2Prefab.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<CharacterBase>().directionLocked = true;
        Invoke("resetAttack", GetAttack2Delay());
    }

    public void resetAttack()
    {
        attack1Prefab.GetComponent<BoxCollider2D>().enabled = false;
        attack2Prefab.GetComponent<BoxCollider2D>().enabled = false;
        attack1Prefab.GetComponent<RogueAttack1Trigger>().playersAlreadyHit.Clear();
        attack2Prefab.GetComponent<RogueAttack2Trigger>().playersAlreadyHit.Clear();
        gameObject.GetComponent<CharacterBase>().directionLocked = false;
    }

    public override float GetAttack1Delay()
    {
        return 0.5f;
    }

    public override float GetAttack2Delay()
    {
        return 1.0f;
    }

    public override bool CanAttack1Hit(int facing)
    {
        var raycast = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0, 0) * facing, Vector2.right * facing, 0.12f);
        return raycast.transform != null ? raycast.transform.tag == "Player" : false;
    }

    public override bool CanAttack2Hit(int facing)
    {
        var raycast = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0, 0) * facing, Vector2.right * facing, 0.18f);
        return raycast.transform != null ? raycast.transform.tag == "Player" : false;
    }
    
    public override float GetAttack1AiWeight()
    {
        return 0.5f;
    }

    public override float GetAttack2AiWeight()
    {
        return 1.0f;
    }
}

using UnityEngine;
using System.Collections;

public class KnightAttacks : AttackBase
{
    private GameObject attack1Prefab;
    private GameObject attack2Prefab;

    AudioSource source1;
    AudioSource source2;

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
        source1 = attack1Prefab.GetComponent<AudioSource>();
        source2 = attack2Prefab.GetComponent<AudioSource>();
        // Make sure that the attack AoEs aren't initially active
        attack1Prefab.GetComponent<BoxCollider2D>().enabled = false;
        attack2Prefab.GetComponent<BoxCollider2D>().enabled = false;
    }
    
    public override void Attack1()
    {
        source1.Play();
        attack1Prefab.GetComponent<BoxCollider2D>().enabled = true;
        Invoke("resetAttack", GetAttack1Delay());
    }
    
    public override void Attack2()
    {
        source2.Play();
        attack2Prefab.GetComponent<BoxCollider2D>().enabled = true;
        Invoke("resetAttack", GetAttack2Delay());
    }
    
    public void resetAttack()
    {
        attack1Prefab.GetComponent<BoxCollider2D>().enabled = false;
        attack2Prefab.GetComponent<BoxCollider2D>().enabled = false;
        attack1Prefab.GetComponent<KnightAttack1Trigger>().playersAlreadyHit.Clear();
        attack2Prefab.GetComponent<KnightAttack2Trigger>().playersAlreadyHit.Clear();
    }

    public override float GetAttack1Delay()
    {
        return 0.5f;
    }

    public override float GetAttack2Delay()
    {
        return 0.8f;
    }

    public override bool CanAttack1Hit(int facing)
    {
        var raycast = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0, 0) * facing, Vector2.right * facing, 0.12f);
        return raycast.transform != null ? raycast.transform.tag == "Player" : false;
    }

    public override bool CanAttack2Hit(int facing)
    {
        var raycast = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0, 0) * facing, Vector2.right * facing, 0.12f);
        return raycast.transform != null ? raycast.transform.tag == "Player" : false;
    }

    public override float GetAttack1AiWeight()
    {
        return 0.5f;
    }

    public override float GetAttack2AiWeight()
    {
        return 0.75f;
    }

}

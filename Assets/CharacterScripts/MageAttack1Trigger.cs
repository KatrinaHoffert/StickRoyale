using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MageAttack1Trigger : MonoBehaviour
{
    List<GameObject> playersAlreadyHit = new List<GameObject>();

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player") && !playersAlreadyHit.Contains(coll.gameObject))
        {
            coll.gameObject.GetComponent<CharacterBase>().Damage(15);
            coll.gameObject.GetComponent<CharacterBase>().DamageForce(gameObject.GetComponentInParent<Transform>().forward * 100);
            playersAlreadyHit.Add(coll.gameObject);
        }
    }
}

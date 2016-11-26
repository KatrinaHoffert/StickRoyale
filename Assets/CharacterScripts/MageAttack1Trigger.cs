using UnityEngine;
using System.Collections;

public class MageAttack1Trigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            coll.gameObject.GetComponent<CharacterBase>().Damage(15);
            coll.gameObject.GetComponent<CharacterBase>().DamageForce(gameObject.GetComponentInParent<Transform>().forward * 100);
        }

        GameObject.Destroy(gameObject);
    }
}

using UnityEngine;
using System.Collections;

public class KnightAttackTrigger : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("Player"))
        {
            Debug.Log("hit");
            coll.gameObject.GetComponent<CharacterBase>().Damage(5);
            coll.gameObject.GetComponent<CharacterBase>().DamageForce(transform.right);
        }
    }
}

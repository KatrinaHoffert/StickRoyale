using UnityEngine;
using System.Collections;

public class KnightAttackTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("Player"))
        {
            Debug.Log("hit");
            coll.gameObject.GetComponent<CharacterBase>().SendMessage("Damage", 5);
            coll.gameObject.GetComponent<CharacterBase>().SendMessage("DamageForce",gameObject.GetComponentInParent<Transform>().forward);
        }
    }
}

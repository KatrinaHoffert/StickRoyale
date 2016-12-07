using UnityEngine;
using System.Collections;

public class HeadCollision : MonoBehaviour {

    PlayerMovement moveScript;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    /// <summary>
    /// Used for detecting if players stack.
    /// </summary>
    void OnTriggerStay2D(Collider2D coll)
    {
        Debug.Log(coll.gameObject.ToString());
        if (coll.gameObject.tag == "Player")
        {
            Debug.Log(coll.gameObject.ToString());
            if(gameObject.GetComponentInParent<Ai>())
            {
                gameObject.GetComponentInParent<Ai>().moveOffPlayer();
            } else
            {
                gameObject.GetComponentInParent<PlayerMovement>().moveOffPlayer();
            }
            
        }
    }
}

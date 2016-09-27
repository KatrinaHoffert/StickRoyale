using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerMovement : NetworkBehaviour {
    Vector2 playerMoveSpeed;
    Vector2 playerStop;

	// Use this for initialization
	void Start () {
        playerMoveSpeed = new Vector2(5f, 0f);
        playerStop = playerMoveSpeed - playerMoveSpeed;
    }
	
	// Update is called once per frame
	void Update () {
        if(!isLocalPlayer)
        {
            return;
        }

	    if(Input.GetKeyDown("d")) {
            GetComponent<MovementScript>().SendMessage("ChangeMoveForce", playerMoveSpeed);
        }
        if(Input.GetKeyUp("d"))
        {
            GetComponent<MovementScript>().SendMessage("ChangeMoveForce", playerStop);
        }
	}
}

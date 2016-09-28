using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerMovement : NetworkBehaviour {
    Vector2 playerMoveSpeedRight;
    Vector2 playerStop;
    Vector2 playerMoveSpeedLeft;

	// Use this for initialization
	void Start () {
        playerMoveSpeedRight = new Vector2(5f, 0f);
        playerStop = playerMoveSpeedRight - playerMoveSpeedRight;
        playerMoveSpeedRight = new Vector2(-5f, 0f);

    }
	
	// Update is called once per frame
	void Update () {
        if(!isLocalPlayer)
        {
            return;
        }

	    if(Input.GetKeyDown("d")) {
            GetComponent<MovementScript>().SendMessage("ChangeMoveForce", playerMoveSpeedRight);
        }
        if(Input.GetKeyUp("d"))
        {
            GetComponent<MovementScript>().SendMessage("ChangeMoveForce", playerStop);
        }
        if (Input.GetKeyDown("a"))
        {
            GetComponent<MovementScript>().SendMessage("ChangeMoveForce", playerMoveSpeedLeft);
        }
        if (Input.GetKeyUp("a"))
        {
            GetComponent<MovementScript>().SendMessage("ChangeMoveForce", playerStop);
        }
    }
}

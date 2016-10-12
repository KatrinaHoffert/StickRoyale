using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {
    public Vector2 playerMoveSpeedRight;
    public Vector2 playerStop;
    public Vector2 playerMoveSpeedLeft;
    public Vector2 jumpForce;
    float direction;
    float vertical;

	// Use this for initialization
	void Start () {
        playerMoveSpeedRight = new Vector2(10f, 0f);
        playerStop = playerMoveSpeedRight - playerMoveSpeedRight;
        playerMoveSpeedLeft = new Vector2(-10f, 0f);
        jumpForce = new Vector2(0f, 50f);


    }
	
	// Update is called once per frame
	void Update () {
        if(Time.frameCount>700)
        {

            if (!gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                return;
            }
        }




        direction = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        if (direction>0) {
            GetComponent<MovementScript>().SendMessage("ChangeMoveForce", playerMoveSpeedRight);
        }
        if(direction==0)
        {
            GetComponent<MovementScript>().SendMessage("ChangeMoveForce", playerStop);
        }
        if (direction<0)
        {
            GetComponent<MovementScript>().SendMessage("ChangeMoveForce", playerMoveSpeedLeft);
        }
        if(vertical>0)
        {
            GetComponent<MovementScript>().SendMessage("Jump", jumpForce);
        }
    }
}

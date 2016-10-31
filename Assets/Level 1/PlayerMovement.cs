using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {
    public Vector2 playerMoveSpeedRight;
    public Vector2 playerStop;
    public Vector2 playerMoveSpeedLeft;
    public Vector2 jumpForce;
    public int max_Jumps;
    public int on_Floor;
    float direction;
    float vertical;
    public int jumpsleft;

    // Use this for initialization
    void Start()
    {
        playerMoveSpeedRight = new Vector2(40f, 0f);
        playerStop = playerMoveSpeedRight - playerMoveSpeedRight;
        playerMoveSpeedLeft = new Vector2(-40f, 0f);
        jumpForce = new Vector2(0f, 30f);
        max_Jumps = 1;

    }
	
	// Update is called once per frame
	void Update () {

        if (!isLocalPlayer)
        {
            return;
        }
        direction = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        if (direction == 0)
        {
            GetComponent<MovementScript>().SendMessage("ChangeMoveForce", playerStop);
        } else if (direction>0) {
            GetComponent<MovementScript>().SendMessage("ChangeMoveForce", playerMoveSpeedRight);
        } else if (direction<0)
        {
            GetComponent<MovementScript>().SendMessage("ChangeMoveForce", playerMoveSpeedLeft);
        }
        if(vertical>0 && jumpsleft>0)
        {
            GetComponent<MovementScript>().SendMessage("Jump", jumpForce);
        }
    }



    void onTriggerEnter2D(Collision2D coll)
    {
        if(coll.gameObject.CompareTag("Floor"))
        {
            jumpsleft = max_Jumps;
        }

    }
}

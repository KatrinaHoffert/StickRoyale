using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AnimationControl : NetworkBehaviour {
    [SyncVar]
    public bool atk1;
    [SyncVar]
    public bool atk2;
    [SyncVar]
    public bool walk;

    bool indexFound = false;
    
    public int playerIndex;

    [SyncVar]
    public bool facingRight;
	// Use this for initialization
	void Start () {

        initialize();
    
       
	}
	
    void initialize()
    {
     
        facingRight = true;
        atk2 = false;

    }

	// Update is called once per frame
	void FixedUpdate () {
        if (!indexFound)
        {
            if(levelscript.players != null)
            {
                indexFound = true;
                for (int i = 0; i < levelscript.players.Length; i++)
                {
                    if (this.GetComponent<PlayerBase>().uniquePlayerId == levelscript.players[i].GetComponent<PlayerBase>().uniquePlayerId)
                    {
                        playerIndex = i;
                        atk2 = false;
                    }
                }
            }
        }
        if (isClient)
        {
            RpcDoAnimations();
        }
        
        if (!isLocalPlayer)
        {
            
            return;
        }
        
        
        ChooseAnimation();
     

    }

    [ClientRpc]
    void RpcDoAnimations()
    {



        if (GetComponent<Animator>() != null)
        {
            //this.GetComponent<Animator>().SetBool("walk", this.walk);
            levelscript.players[playerIndex].GetComponent<Animator>().SetBool("atk2", levelscript.players[playerIndex].GetComponent<AnimationControl>().atk2);
            levelscript.players[playerIndex].GetComponent<Animator>().SetBool("atk1", levelscript.players[playerIndex].GetComponent<AnimationControl>().atk1);
            //this.GetComponent<Animator>().SetBool("atk1", this.atk1);
        }
           
   

    }
  
    [Command]
    void CmdAtk2(bool value)
    {
        this.atk2 = value;
    }
    [Command]
    void CmdAtk1(bool value)
    {
        this.atk1 = value;
    }
    [Command]
    void CmdFacingRight(bool value)
    {
        this.facingRight = value;
    }

    void ChooseAnimation()
    {
        if (Input.GetKey(KeyCode.Z))
        {

            //this.GetComponent<Animator>().SetBool("atk1", true);
            /*
            if (!atk1)
            {
                atk1=true;
            }
            */
            CmdAtk1(true);
        }
        if (Input.GetKey(KeyCode.X))
        {

            //this.GetComponent<Animator>().SetBool("atk2", true);
            //if (!atk2[playerIndex])
            //{

            //atk2[playerIndex] = true;
            //CmdChangeArrayValue(atk2, true, playerIndex);
            //atk2 = true;
            CmdAtk2(true);

        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
      
            //this.GetComponent<Animator>().SetBool("walk", true);
            if (!facingRight)
            {
                //facingRight = true;
                CmdFacingRight(true);
                //this.transform.Rotate(0, -180, 0);
            }
            if (!walk)
            {
                walk = true;
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //this.resetAll();
            //this.GetComponent<Animator>().SetBool("walk", true);
            if (facingRight)
            {
                //facingRight = false;
                CmdFacingRight(false);
                //this.transform.Rotate(0, 180, 0);
            }
            if (!walk)
            {
                walk = true;
            }
        }
       

        if (!Input.anyKey)
        {
            this.resetAll();
        }
    }

    void resetAll()
    {
        if (this.GetComponent<Animator>() != null)
        {
            CmdAtk1(false);
            CmdAtk2(false);
            this.GetComponent<Animator>().SetBool("atk1", false);
            this.GetComponent<Animator>().SetBool("atk2", false);
            this.GetComponent<Animator>().SetBool("walk", false);
            this.GetComponent<Animator>().SetBool("fall", false);
            this.GetComponent<Animator>().SetBool("die", false);
            this.GetComponent<Animator>().SetBool("idle", false);
            this.GetComponent<Animator>().SetBool("jump", false);
            this.GetComponent<Animator>().SetBool("block", false);
        }
        
    }
}

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class testPlayerScript : NetworkBehaviour
{
    [SyncVar]
    public int hitpoints;
    void Start()
    {
        hitpoints = 100;
   
    }
    void FixedUpdate()
    {
        
        if (!isLocalPlayer)
        {
            return;
        }
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var y = Input.GetAxis("Vertical") * Time.deltaTime * 300.0f;

  
        transform.Translate(x, y, 0);

        CmdDie();
       

      


    }
    /// <summary>
    /// Die if below certain y
    /// command is used to send data from client to server
    /// </summary>
    [Command]
    void CmdDie()
    {
        if (transform.position.y < -300)
        {

            this.hitpoints = 0;

        }
    }

    [Command]
    void CmdDamaged(int damage)
    {
        this.hitpoints = this.hitpoints - damage;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //CmdDamaged(1);
        if(col.gameObject.tag == "Player")
        {
            CmdDamaged(1);
        }
    }
}
using UnityEngine;
using UnityEngine.Networking;
public class testPlayerScript : NetworkBehaviour
{
    void Start()
    {

    }
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var y = Input.GetAxis("Vertical") * Time.deltaTime * 300.0f;

  
        transform.Translate(x, y, 0);
    }
}
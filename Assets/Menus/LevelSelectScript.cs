using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class LevelSelectScript : MonoBehaviour
{
    public void Level1Selected()
    {
        SceneManager.LoadScene("Level1");
    }
    
    public void Level2Selected()
    {
        SceneManager.LoadScene("Level2");
    }
}

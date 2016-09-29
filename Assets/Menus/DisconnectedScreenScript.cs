using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DisconnectedScreenScript : MonoBehaviour
{
    public void ContinueButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

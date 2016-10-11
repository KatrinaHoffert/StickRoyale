using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DisconnectedScreenScript : MonoBehaviour
{
    void Start()
    {
        // Attempt cleanup of items that often are left behind.
        while (GameObject.Find("ControlSlots") != null) DestroyImmediate(GameObject.Find("ControlSlots"));
        while (GameObject.Find("NetworkManager") != null) DestroyImmediate(GameObject.Find("NetworkManager"));
    }

    /// <summary>
    /// Called when the continue button is clicked. Just takes us back to the main menu.
    /// </summary>
    public void ContinueButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

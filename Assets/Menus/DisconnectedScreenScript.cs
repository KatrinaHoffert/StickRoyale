using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DisconnectedScreenScript : MonoBehaviour
{
    void Start()
    {
        while (GameObject.Find("ControlSlots") != null) DestroyImmediate(GameObject.Find("ControlSlots"));
        while (GameObject.Find("NetworkManager") != null) DestroyImmediate(GameObject.Find("NetworkManager"));
    }

    public void ContinueButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

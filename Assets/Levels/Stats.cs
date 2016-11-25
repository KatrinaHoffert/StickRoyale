using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Stores stats that are accumulated during the game.
/// </summary>
public class Stats : MonoBehaviour
{
    /// <summary>
    ///  The player who won the match. State is undefined until we reach the PostGameScreen.
    /// </summary>
    public string winner;
}

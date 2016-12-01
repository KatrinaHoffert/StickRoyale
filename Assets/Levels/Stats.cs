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

    /// <summary>
    /// Per player kill counts.
    /// </summary>
    public int[] kills = new int[4];

    /// <summary>
    /// Per player fall counts.
    /// </summary>
    public int[] falls = new int[4];

    /// <summary>
    /// Adds a kill for this player.
    /// </summary>
    /// <param name="player">The player who got the kill.</param>
    public void AddKill(GameObject player)
    {
        int playerNumber = player.name[player.name.Length - 1] - '0';
        ++kills[playerNumber];
    }

    /// <summary>
    /// Adds a fall for this player.
    /// </summary>
    /// <param name="player">The player who got the fall.</param>
    public void AddFall(GameObject player)
    {
        int playerNumber = player.name[player.name.Length - 1] - '0';
        ++falls[playerNumber];
    }
}

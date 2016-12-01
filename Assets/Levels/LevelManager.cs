using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// The LevelManager performs all the fundamental actions that are needed for any particular level to work.
/// All this functionality is necessary no matter what the level is. Specifically, it'll create the characters,
/// attach appropriate scripts to each, handle the spawning (initially and on death), and handle the checking
/// of victory conditions (and thus the progression to the next screen).
/// </summary>
public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// All the players, to avoid having to query for these unnecessarily.
    /// </summary>
    public static GameObject[] players;

    /// <summary>
    /// The spawn points that this level has.
    /// </summary>
    private GameObject[] spawnPoints;

    /// <summary>
    /// The next game time that the spawn point is available. Facilitates cooldowns so that multiple players
    /// aren't using a spawn point and so players aren't respawning too frequently at a spawn point.
    /// </summary>
    private float[] spawnPointAvailabilityTime;

    /// <summary>
    /// Cooldown time for a spawn point to be used. Only violated if all spawn points are on cooldown.
    /// </summary>
    private const float spawnPointCooldown = 3.0f;

    /// <summary>
    /// Stores the indicator objects for each player.
    /// </summary>
    private GameObject[] indicators;

    /// <summary>
    /// Stores the hitbar objects for each player. The redbar is the object stored here and it has a child,
    /// greenbar, that shrinks as HP is lost.
    /// </summary>
    private GameObject[] hitbars;

    /// <summary>
    /// Text labels used for the display of how many lives each character has.
    /// </summary>
    private GameObject[] livesText;

    private static Stats stats;
   
    void Start () {
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        spawnPointAvailabilityTime = new float[spawnPoints.Length];

        // Get the GUI elements for displaying lives before players are created so that they
        // can be enabled only for players that exist.
        Instantiate(Resources.Load("LivesOverlay"));
        livesText = new GameObject[]
        {
            GameObject.Find("Player0Lives"),
            GameObject.Find("Player1Lives"),
            GameObject.Find("Player2Lives"),
            GameObject.Find("Player3Lives")
        };

        // Create player objects
        var controlSlots = GameObject.Find("ControlSlots").GetComponent<ControlSlotsScript>().slots;
        players = new GameObject[4];
        for(int i = 0; i < controlSlots.Length; ++i)
        {
            if(controlSlots[i].controlType != ControlType.Closed)
            {
                players[i] = Instantiate((GameObject)Resources.Load("CharacterPrefabs/" + controlSlots[i].chosenCharacter));

                // Name the objects for debugging
                if(controlSlots[i].controlType == ControlType.Player)
                {
                    players[i].name = "P"+i;
                    players[i].AddComponent<PlayerMovement>();
                }
                else
                {
                    players[i].name = "AI" + i;
                    players[i].AddComponent<Ai>();
                }

                livesText[i].GetComponent<Text>().enabled = true;
                livesText[i].GetComponent<Text>().text = "Player " + i + " lives: " + players[i].GetComponent<CharacterBase>().lives;
            }
        }

        // Player labels are prefabs, this is easier than to try to use the canvas to do this
        indicators = new GameObject[players.Length];

        // Hitbars are simply two sprites, a green box on top of a red box
        hitbars = new GameObject[players.Length];

        for (int i=0; i < players.Length; i++)
        {
            if (CharacterSelectMenuScript.controlSlots[i].controlType != ControlType.Closed)
            {
                // Puts players in spawn points (assumes there's at least 4 spawn points)
                players[i].transform.position = pickSpawnpoint().transform.position;

                // Load the appropriate prefab for the player's indicator
                indicators[i] = Instantiate((GameObject)Resources.Load("PlayerIndicators/Player" + i + "Indicator"));
                indicators[i].name = "Indicator" + i;
                indicators[i].transform.localScale = new Vector3(1, 1, 1);
                hitbars[i] = Instantiate((GameObject)Resources.Load("PlayerIndicators/HealthBar"));
                hitbars[i].name = "HealthBar" + i;
            }
        }

        // Create the game stats storing object
        var statsObject = new GameObject("Stats");
        stats = statsObject.AddComponent<Stats>();
        stats.matchStartTime = Time.time;
        DontDestroyOnLoad(statsObject);
    }

    /// <summary>
    /// Picks a spawn point to use. Tries to find a random one off cooldown if possible. Otherwise
    /// will pick the one with the shortest cooldown. Will set the cooldown of the spawn point that
    /// gets returned.
    /// </summary>
    /// <returns>The spawnpoint the player should spawn at.</returns>
    private GameObject pickSpawnpoint()
    {
        var availableSpawnPoints = 0;
        foreach(var availabilityTime in spawnPointAvailabilityTime)
        {
            if (availabilityTime + spawnPointCooldown <= Time.time) ++availableSpawnPoints;
        }

        // No spawn points? Pick the oldest one
        int spawnPointToUse = -1;
        if (availableSpawnPoints == 0)
        {
            float minTime = spawnPointAvailabilityTime[0];
            spawnPointToUse = 0;
            for (int i = 1; i < spawnPointAvailabilityTime.Length; ++i)
            {
                if (spawnPointAvailabilityTime[i] < minTime)
                {
                    minTime = spawnPointAvailabilityTime[i];
                    spawnPointToUse = i;
                }
            }
            Debug.Log("No spawn points off cooldown, using " + spawnPoints[spawnPointToUse].name);
        }
        // Otherwise pick a random spawn point from those available
        else
        {
            // This is an index skipping the spawn points that are on cooldown
            var spawnPointPseudoIndex = UnityEngine.Random.RandomRange(0, availableSpawnPoints - 1);
            int spawnPointOffCooldownIndex = 0;
            for (int i = 0; i < spawnPointAvailabilityTime.Length; ++i)
            {
                if (spawnPointAvailabilityTime[i] + spawnPointCooldown > Time.time) continue;
                if (spawnPointOffCooldownIndex == spawnPointPseudoIndex) spawnPointToUse = i;
                ++spawnPointOffCooldownIndex;
                // Working on getting it to pick furthest spawn point from players
                /*GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                float[] distanceToSpawnPoint = new float[players.Length];
                foreach(GameObject item in players)
                {
                    distanceToSpawnPoint[i] = Vector3.Distance(item.transform.position, spawnPoints[i].transform.position);
                }
                float max = distanceToSpawnPoint[1];
                GameObject furthestPoint = spawnPoints[1];
                for(int j = 1; j < players.Length;j++)
                {
                    max = Mathf.Max(max, distanceToSpawnPoint[j]);
                    if(max==distanceToSpawnPoint[j])
                    {
                        spawnPointToUse = j;
                    }
                }*/
                
            }
        }

        // Make sure we reset the cooldown on the spawn point we're using
        spawnPointAvailabilityTime[spawnPointToUse] = Time.time + spawnPointCooldown;
        return spawnPoints[spawnPointToUse];
    }

    /// <summary>
    /// Respawns if player is dead.
    /// </summary>
    /// <param name="player">The player to check if we should respawn.</param>
    private void RespawnIfDead(GameObject player)
    {
        int playerNumber = player.name[player.name.Length - 1] - '0';
        var playerBase = player.GetComponent<CharacterBase>();
        if (playerBase.currentHitpoints <= 0)
        {
            playerBase.Die();

            livesText[playerNumber].GetComponent<Text>().text = "Player " + playerNumber + " lives: " + playerBase.lives;

            if (playerBase.lives > 0)
            {
                player.transform.position = pickSpawnpoint().transform.position;
                player.GetComponent<Rigidbody2D>().velocity = new Vector2();
            }
            else
            {
                Debug.Log(player.name + " is out of lives");
                Destroy(player);
                Destroy(GameObject.Find("Indicator" + playerNumber));
                Destroy(GameObject.Find("HealthBar" + playerNumber));
                VictoryConditionCheck();
            }
        }
    }

    /// <summary>
    /// Checks if the game's victory condition (namely being the last player standing) has
    /// been met. If so, transitions us to the stats screen.
    /// </summary>
    private void VictoryConditionCheck()
    {
        int numPlayersAlive = 0;
        string playerAliveName = "";
        for (int i = 0; i < players.Length; ++i)
        {
            if (players[i] == null) continue;

            var playerBase = players[i].GetComponent<CharacterBase>();
            if (playerBase.lives > 0)
            {
                ++numPlayersAlive;
                playerAliveName = "Player " + i;
            }
        }

        if (numPlayersAlive == 1)
        {
            stats.winner = playerAliveName;
            stats.matchEndTime = Time.time;
            SceneManager.LoadScene("PostGameScreen");
        }
        // Special case that could possibly happen
        else if(numPlayersAlive == 0)
        {
            stats.winner = "";
            stats.matchEndTime = Time.time;
            SceneManager.LoadScene("PostGameScreen");
        }
    }
    
    void FixedUpdate () {
        for(int i = 0; i < players.Length; i++)
        {
            if (players[i] == null) continue;

            RespawnIfDead(players[i]);

            // Update player label position
            indicators[i].transform.position = players[i].transform.position + new Vector3(0, 1.6f, 0);

            // Update hitbar location and size
            int playerHp = players[i].GetComponent<CharacterBase>().currentHitpoints;
            int playerMaxHp = players[i].GetComponent<CharacterBase>().maxHitpoints;
            hitbars[i].transform.position = players[i].transform.position + new Vector3(-0.8f, 0.9f, 0);
            hitbars[i].transform.FindChild("RemainingHealthBar").transform.localScale = new Vector3((playerHp / (float)playerMaxHp), 0.6f, 1);
        }
	}
}

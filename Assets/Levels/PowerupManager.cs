using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

/// <summary>
/// Manages spawning and placement of powerups.
/// </summary>
public class PowerupManager : MonoBehaviour
{
    /// <summary>
    /// All powerups and their spawn weighting. Higher numbers mean a proportionately higher
    /// chance of being chosen.
    /// </summary>
    private PowerupSpawnRate[] powerups = new PowerupSpawnRate[]
    {
        new PowerupSpawnRate("PowerupBurger", 1.0f),
        new PowerupSpawnRate("PowerupSteroids", 0.35f),
        new PowerupSpawnRate("PowerupInvincible", 0.5f),
        new PowerupSpawnRate("PowerupFire", 0.5f)
    };

    /// <summary>
    /// Mean time between powerup spawns.
    /// </summary>
    private float meanSpawnTime = 10f;

    /// <summary>
    /// Standard deviation in time between powerup spawns.
    /// </summary>
    private float stdDevSpawnTime = 5f;

    /// <summary>
    /// Next time that a powerup should spawn.
    /// </summary>
    private float nextSpawnTime;

    void Start()
    {
        nextSpawnTime = Time.time + RandomNormal(meanSpawnTime, stdDevSpawnTime);
    }

    void Update()
    {
        if(Time.time >= nextSpawnTime)
        {
            // Find all our floors and pick a random one to spawn it on
            var floors = GameObject.FindGameObjectsWithTag("Floor");
            var floorChoice = floors[UnityEngine.Random.Range(0, floors.Length - 1)];

            // Pick a point on its surface (assuming that the platform is flat)
            var floorCollider = floorChoice.GetComponent<BoxCollider2D>();
            var floorSize = floorCollider.bounds.size;
            var floorCenter = floorCollider.bounds.center;

            // Pick a random powerup to spawn
            var totalWeights = powerups.Sum(record => record.spawnWeighting);
            var random = UnityEngine.Random.Range(0, totalWeights);
            var runningSum = 0f;
            string powerupName = "";
            foreach(var record in powerups)
            {
                //Debug.Log("rand: " + random + ");
                runningSum += record.spawnWeighting;
                if(random <= runningSum)
                {
                    powerupName = record.powerupName;
                    break;
                }
            }

            var powerup = Instantiate((GameObject)Resources.Load("PowerupPrefabs/" + powerupName));
            var powerupSize = powerup.GetComponent<BoxCollider2D>().bounds.size;

            // Pick a random point on the floor's horizontal range, accounting for the powerup's size
            var xOffset = UnityEngine.Random.Range(-floorSize.x / 2 + powerupSize.x / 2, floorSize.x / 2 - powerupSize.x / 2);
            var yOffset = floorSize.y / 2 + powerupSize.y / 2;
            powerup.transform.position = floorCenter + new Vector3(xOffset, yOffset, 0);

            nextSpawnTime = Time.time + RandomNormal(meanSpawnTime, stdDevSpawnTime);

            Debug.Log("Spawned " + powerup.name + " at pos " + powerup.transform.position + " at time " + Time.time);
        }
    }

    /// <summary>
    /// Implementation of a Box-Muller transform for getting a normally distributed random number.
    /// </summary>
    /// <param name="mean">The distribution mean.</param>
    /// <param name="stDev">The distribution standard deviation.</param>
    /// <returns>A random number.</returns>
    private float RandomNormal(float mean, float stDev)
    {
        float u1 = UnityEngine.Random.value;
        float u2 = UnityEngine.Random.value;
        float randStdNormal = (float)(Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2));
        return mean + stDev * randStdNormal;
    }

    private class PowerupSpawnRate
    {
        public string powerupName;
        public float spawnWeighting;

        public PowerupSpawnRate(string powerupName, float spawnWeighting)
        {
            this.powerupName = powerupName;
            this.spawnWeighting = spawnWeighting;
        }
    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages spawning and placement of powerups.
/// </summary>
public class PowerupManager : MonoBehaviour
{
    PowerupSpawnRate[] powerups = new PowerupSpawnRate[]
    {
        new PowerupSpawnRate("PowerupBurger", 1.0f)
    };

    void Update()
    {
        // TODO: Place powerups
    }

    private class PowerupSpawnRate
    {
        string powerupName;
        float spawnWeighting;

        public PowerupSpawnRate(string powerupName, float spawnWeighting)
        {
            this.powerupName = powerupName;
            this.spawnWeighting = spawnWeighting;
        }
    }
}
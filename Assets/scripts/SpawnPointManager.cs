using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    // Singleton Instance
    public static SpawnPointManager Instance { get; private set; }

    [Tooltip("All the Transforms you want players to be able to spawn at")]
    public List<Transform> spawnPoints;

    private List<Transform> available;

    private void Awake()
    {
        // Ensuring only one instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instance if one already exists
            return;
        }

        // Initialize available spawn points
        Refill();
    }

    // Refill the spawn points
    private void Refill()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError($"[{nameof(SpawnPointManager)}] No spawnPoints assigned in inspector!");
            available = new List<Transform>();
        }
        else
        {
            available = new List<Transform>(spawnPoints);
        }
    }

    // Get a random spawn point
    public Transform GetRandomSpawnPoint()
    {
        if (available == null || available.Count == 0)
            Refill();

        if (available.Count == 0)
            return null;

        int i = Random.Range(0, available.Count);
        var pt = available[i];
        available.RemoveAt(i);
        return pt;
    }
}
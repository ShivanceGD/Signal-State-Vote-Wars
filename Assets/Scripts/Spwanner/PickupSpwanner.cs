using UnityEngine;
using System.Collections.Generic;

public class PickupSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject fuelPrefab;
    public GameObject speedPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Spawn Settings")]
    public float minSpawnInterval = 3f;
    public float maxSpawnInterval = 7f;

    private List<GameObject> spawnedPickups = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private System.Collections.IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));

            SpawnPickup();
        }
    }

    private void SpawnPickup()
    {
        List<Transform> availablePoints = new List<Transform>();

        // Find all spawn points that are empty
        foreach (var point in spawnPoints)
        {
            if (!IsSpawnPointOccupied(point))
            {
                availablePoints.Add(point);
            }
        }

        // If there are no free points, do nothing
        if (availablePoints.Count == 0)
        {
            return;
        }

        // Pick a random free point
        Transform spawnPoint = availablePoints[Random.Range(0, availablePoints.Count)];

        // Randomly choose what to spawn (Fuel or Speed)
        GameObject prefabToSpawn = (Random.value > 0.5f) ? fuelPrefab : speedPrefab;

        // Spawn it
        GameObject spawnedPickup = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);

        // Add to the list for tracking
        spawnedPickups.Add(spawnedPickup);
    }

    private bool IsSpawnPointOccupied(Transform point)
    {
        foreach (var pickup in spawnedPickups)
        {
            if (pickup != null)
            {
                float distance = Vector3.Distance(pickup.transform.position, point.position);
                if (distance < 1f) // Small threshold to consider "occupied"
                {
                    return true;
                }
            }
        }
        return false;
    }
}

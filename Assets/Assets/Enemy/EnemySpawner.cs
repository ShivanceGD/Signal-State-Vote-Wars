using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
///Should spawn determines if the spawn point should be spawning.
public struct SpawnPoint
{
    public Transform SpawnLocation;
    public bool ShouldSpawn;
}

[Serializable]
public struct WantedLevel
{
    [HideInInspector]public float RangeLow, RangeHigh; //Determines between what range the wanted level is active

    public int MaxActiveShips; //Max ships ofcom can dispatch

    public int Stars;

    public float MinSpawnTimeSeconds, MaxSpawnTimeSeconds; //Time between ships are spawned
}

public class EnemySpawner : MonoBehaviour
{
    public WantedLevel[] WantedLevels = new WantedLevel[6];
    public SpawnPoint[] SpawnPoints;
    public GameObject[] Enemies;
    //public Transform[] PatrolPoints;

    WantedLevel currentWantedLevel;

    bool intitalDelay = true;

    float lastTimeSpawnedShip;
    float nextDelayInSpawningShip;

	void Start ()
    {
        lastTimeSpawnedShip = Time.time;
        nextDelayInSpawningShip = UnityEngine.Random.Range(currentWantedLevel.MinSpawnTimeSeconds, currentWantedLevel.MaxSpawnTimeSeconds);
    }

	void Update ()
    {
        updateCurrentWantedLevel();

        int amountOfEnemyShips = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (amountOfEnemyShips <= currentWantedLevel.MaxActiveShips)
        {
            if (lastTimeSpawnedShip + nextDelayInSpawningShip <= Time.time)
            {
                lastTimeSpawnedShip = Time.time;
                nextDelayInSpawningShip = UnityEngine.Random.Range(currentWantedLevel.MinSpawnTimeSeconds, currentWantedLevel.MaxSpawnTimeSeconds);

                //Instanciate ship
                SpawnPoint pickedSpawnPoint = pickSpawnPoint();
                //if (pickedSpawnPoint.SpawnLocation != null)
                //{
                    GameObject spawnedBoat = Instantiate(pickEnemy(), pickedSpawnPoint.SpawnLocation.position, pickedSpawnPoint.SpawnLocation.rotation);
                    //spawnedBoat.GetComponent<EnemyMover>().SpawnPoint = pickedSpawnPoint.SpawnLocation;
                if (!spawnedBoat)
                {
                    Debug.Log("Error");
                }
                if (!spawnedBoat.GetComponent<EnemyMover>())
                {
                    Debug.Log("Enemy mover not found in boat.");
                }
                //}
                //else
                //{
                //    Debug.LogError("Error: All spawn locations have a null transform.");
                //}
            }
        }
	}

    //Make current wanted level
    void updateCurrentWantedLevel()
    {
        foreach (var wantedLevel in WantedLevels)
        {
            /*if (GameVariables.Notoriety >= wantedLevel.RangeLow &&
                GameVariables.Notoriety <= wantedLevel.RangeHigh)
            {
                currentWantedLevel = wantedLevel;
                break;
            }*/
        }
    }

    //Useless as it just would spawn too may ships with a backlog
    //IEnumerator shipSpawner()
    //{
    //    if (intitalDelay)
    //    {
    //        yield return new WaitForSeconds(10f); //Wait 10s before spawning first enemy
    //        intitalDelay = false;
    //    }
        
        
    //    //Instanciate ship
    //    SpawnPoint pickedSpawnPoint = pickSpawnPoint();
    //    if (pickedSpawnPoint.SpawnLocation != null)
    //    {
    //        GameObject spawnedBoat = Instantiate(pickEnemy(), pickedSpawnPoint.SpawnLocation.position, pickedSpawnPoint.SpawnLocation.rotation);
    //        spawnedBoat.GetComponent<EnemyMover>().SpawnPoint = pickedSpawnPoint.SpawnLocation;
    //    }
    //    else
    //    {
    //        Debug.LogError("Error: All spawn locations have a null transform."); 
    //    }

        
    //    //Wait to insanciate next
    //    yield return new WaitForSeconds(UnityEngine.Random.Range(currentWantedLevel.MinSpawnTimeSeconds, currentWantedLevel.MaxSpawnTimeSeconds));
    //}

    GameObject pickEnemy()
    {
        return Enemies[UnityEngine.Random.Range(0, Enemies.Length)];
    }

    SpawnPoint pickSpawnPoint()
    {
        ////Pick an active spawn point
        //SpawnPoint pickedSpawnPoint;
        //for (int i = 0; i < SpawnPoints.Length; i++)
        //{
        //    pickedSpawnPoint = SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Length - 1)];
        //    if (pickedSpawnPoint.ShouldSpawn)
        //    {
        //        return pickedSpawnPoint;
        //    }
        //}
        // return new SpawnPoint();
        return SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Length)]; //nothing has been found.
    }
}

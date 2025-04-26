using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMover : MonoBehaviour
{
    public float delayBeforeGoingBackToBase = 5f;
    public int MinPatrolPoints = 5, MaxPatrolPoints = 10;
    public float toleranceAroundTarget = 2.0f;
    public float ScrambleTime = 5f;

    private int patrolsLeftBeforeGoingBackToBase;
    Transform currentPatrolTarget;

    [HideInInspector] public Transform SpawnPoint; //where the boat will return to once its going to return
    Transform[] patrolPoints;

    private Transform player;
    private NavMeshAgent navMesh;

    private bool goingToPlayer; //should the ship be moving?
    private bool goingBackToBase;
    private bool goingToPatrol;
    private bool goingBackwards;
    private float timeStoppedNavigatingTowardsPlayer = 0f;

    Transform scrambledTarget;
    float timeScrambleTriggered;

    public void Scramble()
    {
        scrambledTarget.position = new Vector3(-navMesh.destination.x * 10, navMesh.destination.y, -navMesh.destination.z * 10); //*10 to make it go further away
        timeScrambleTriggered = Time.time;
        goingBackwards = true;
    }

	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (!player)
        {
            Debug.LogError("Player Not found");
        }

        navMesh = GetComponent<NavMeshAgent>();
        if (!navMesh)
        {
            Debug.LogError("Nav mesh not found");
        }

        //Setup patrol points
        GameObject[] patrolGameObjects = GameObject.FindGameObjectsWithTag("PatrolPoint");
        patrolPoints = new Transform[patrolGameObjects.Length];
        for (int i = 0; i < patrolGameObjects.Length; i++)
        {
            patrolPoints[i] = patrolGameObjects[i].transform;
        }

        if (patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points set. Tag any patrol points PatrolPoint.");
        }

        //Spawn points
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        Debug.Log(spawnPoints.Length);
        SpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
        if (!SpawnPoint || spawnPoints.Length == 0)
        {
            Debug.Log("Spawn point fault");
        }
        //foreach (var gameObject in GameObject.FindGameObjectsWithTag("PatrolPoint"))
        //{
        //    patrolPoints.Add(gameObject.transform);
        //}


        patrolsLeftBeforeGoingBackToBase = Random.Range(MinPatrolPoints, MaxPatrolPoints);
        pickNewPatrolTarget();
        goingToPlayer = false;
        goingBackToBase = false;
        goingBackwards = false;
        goingToPatrol = true;
    }
	

	void Update ()
    {
        if (goingBackwards)
        {
            navMesh.SetDestination(scrambledTarget.position);
            if (timeScrambleTriggered + ScrambleTime < Time.time)
            {
                goingBackwards = false;
            }
        }
        else
        {
            if (goingToPlayer)
            {
                navMesh.isStopped = false;
                navMesh.SetDestination(player.position);
                //Debug.Log("Navigating? " + !navMesh.isStopped + ". Destination: " + navMesh.destination);
            }
            else if (!goingToPlayer)
            {
                navMesh.isStopped = true;
                if (timeStoppedNavigatingTowardsPlayer + delayBeforeGoingBackToBase >= Time.time) //ignore this
                {
                    goingToPatrol = true;
                    navMesh.enabled = true;
                }
            }

            if (goingToPatrol)
            {
                navMesh.isStopped = false;
                if (!currentPatrolTarget)
                    pickNewPatrolTarget();
                navMesh.SetDestination(currentPatrolTarget.position);

                Collider[] collisions = Physics.OverlapSphere(currentPatrolTarget.position, toleranceAroundTarget);
                foreach (var collision in collisions)
                {
                    if (collision.tag == "Enemy" && collision.gameObject == this.gameObject)
                    {
                        pickNewPatrolTarget();
                        navMesh.SetDestination(currentPatrolTarget.position);
                        Debug.Log("Reached destination, setting to new one at " + currentPatrolTarget.position);
                        patrolsLeftBeforeGoingBackToBase--;
                    }
                }

                //if (Vector3.Distance(transform.position, SpawnPoint.position) <= toleranceAroundTarget)
                //{
                //    pickNewPatrolTarget();
                //    navMesh.SetDestination(currentPatrolTarget.position);
                //    Debug.Log("Reached destination, setting to new one at " + currentPatrolTarget.position);
                //    patrolsLeftBeforeGoingBackToBase--;
                //}

                if (patrolsLeftBeforeGoingBackToBase <= 0)
                {
                    goingBackToBase = true;
                    Debug.Log("No more patrolling, going back to base...");

                }
            }
        }

        //needs to be seperate if
        if (goingBackToBase)
        {
            navMesh.isStopped = false;
            navMesh.SetDestination(SpawnPoint.position);

            Collider[] collisions = Physics.OverlapSphere(SpawnPoint.position, toleranceAroundTarget);
            foreach (var collision in collisions)
            {
                if (collision.tag == "Enemy" && collision.gameObject == this.gameObject)
                {
                    Destroy(this.gameObject, 1.0f);
                    Debug.Log("Reached spawn.");
                }
            }

            //Debug.Log("Navigating? " + !navMesh.isStopped + ". Destination: " + navMesh.destination);
            //if (Vector3.Distance(transform.position, SpawnPoint.position) <= toleranceAroundTarget)

            //{
            //    Destroy(this.gameObject, 1.0f);
            //    Debug.Log("Reached spawn.");
            //}
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Infuence") //note spelling
        {
            navMesh.isStopped = false;
            goingToPlayer = true;
            goingToPatrol = false;
            goingBackToBase = false;
            Debug.Log("Enemy entering influcence of transmission.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Infuence")
        {
            goingToPlayer = false;
            timeStoppedNavigatingTowardsPlayer = Time.time;
            Debug.Log("Enemy exiting influcence of transmission.");
        }
    }

    private void pickNewPatrolTarget()
    {
        if (patrolPoints.Length > 0)
        {
            currentPatrolTarget = patrolPoints[Random.Range(0, patrolPoints.Length)];
        }
        else
        {
            Debug.LogError("No patrol points set.");
        }
    }
}

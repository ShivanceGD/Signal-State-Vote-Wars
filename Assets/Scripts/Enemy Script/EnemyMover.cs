using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class EnemyMover : MonoBehaviour
{
    [Header("Patrolling Settings")]
    public float patrolTolerance = 2f;
    public int minPatrols = 3;
    public int maxPatrols = 6;

    [Header("Chase Settings")]
    public float chaseRange = 10f;
    public float chaseCooldown = 5f;

    [Header("Escape Settings")]
    public float scrambleDuration = 5f;

    [Header("References")]
    public Transform[] patrolPoints;
    public AudioClip chaseSound;

    private NavMeshAgent agent;
    private Transform player;
    private Transform basePoint;
    private AudioSource audioSource;

    private bool chasingPlayer = false;
    private bool returningToBase = false;
    private bool scrambling = false;

    private float timeStoppedChasing = 0f;
    private float scrambleStartTime = 0f;

    private Transform currentPatrolTarget;
    private int patrolsRemaining;

    private Vector3 scrambledTarget;

    // New variables for "kill if near" logic
    private float nearPlayerTimer = 0f;
    private bool playerIsDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (patrolPoints.Length == 0 || player == null)
        {
            Debug.LogError("Missing patrol points or player.");
            enabled = false;
            return;
        }

        basePoint = patrolPoints[Random.Range(0, patrolPoints.Length)];
        patrolsRemaining = Random.Range(minPatrols, maxPatrols);
        PickNewPatrolTarget();
    }

    void Update()
    {
        if (scrambling)
        {
            agent.SetDestination(scrambledTarget);
            if (Time.time - scrambleStartTime > scrambleDuration)
                scrambling = false;
            return;
        }

        if (player == null || playerIsDead) return; // Don't continue if player is dead or missing

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            chasingPlayer = true;
            returningToBase = false;
            agent.SetDestination(player.position);

            if (!audioSource.isPlaying && chaseSound != null)
            {
                audioSource.clip = chaseSound;
                audioSource.Play();
            }

            // Check if close enough to "kill"
            if (distanceToPlayer <= 2.1f)
            {
                nearPlayerTimer += Time.deltaTime;
                if (nearPlayerTimer >= 2f)
                {
                    KillPlayer();
                }
            }
            else
            {
                nearPlayerTimer = 0f; // Reset if moves away
            }
        }
        else if (chasingPlayer)
        {
            chasingPlayer = false;
            timeStoppedChasing = Time.time;
            audioSource.Stop();
            nearPlayerTimer = 0f;
        }

        if (!chasingPlayer && Time.time - timeStoppedChasing > chaseCooldown)
        {
            if (patrolsRemaining > 0)
                Patrol();
            else
                ReturnToBase();
        }

        if (returningToBase && ReachedDestination(basePoint.position))
        {
            Destroy(gameObject, 1f);
        }
    }

    void Patrol()
    {
        if (ReachedDestination(currentPatrolTarget.position))
        {
            patrolsRemaining--;
            PickNewPatrolTarget();
        }
        agent.SetDestination(currentPatrolTarget.position);
    }

    void ReturnToBase()
    {
        returningToBase = true;
        agent.SetDestination(basePoint.position);
    }

    bool ReachedDestination(Vector3 target)
    {
        return Vector3.Distance(transform.position, target) <= patrolTolerance;
    }

    void PickNewPatrolTarget()
    {
        currentPatrolTarget = patrolPoints[Random.Range(0, patrolPoints.Length)];
    }

    public void Scramble()
    {
        scrambling = true;
        scrambleStartTime = Time.time;
        Vector3 directionAway = (transform.position - player.position).normalized;
        scrambledTarget = transform.position + directionAway * 20f;
    }

    private void KillPlayer()
    {
        Debug.Log("Player died!");
        playerIsDead = true;

        // Example action: destroy the player GameObject
        Destroy(player.gameObject);

        // Or trigger a "PlayerDead" animation, screen effect, GameOver panel etc
    }
}

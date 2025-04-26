using UnityEngine;

public class HouseManager : MonoBehaviour
{
    private bool hasDecided = false;
    private bool playerNearby = false;

    private float influenceTimer;
    private float influenceDuration;

    [SerializeField] private float minInfluenceTime = 2f;
    [SerializeField] private float maxInfluenceTime = 5f;

    public static int VoteCount = 0;

    [Header("Sounds")]
    public AudioClip successSound;  // Drag your "influenced" sound here
    public AudioClip failSound;     // Drag your "resisted" sound here
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add one if missing
        }
    }

    private void Update()
    {
        if (!hasDecided && playerNearby)
        {
            influenceTimer -= Time.deltaTime;

            if (influenceTimer <= 0)
            {
                TryToInfluence();
            }
        }
    }

    private void TryToInfluence()
    {
        hasDecided = true; // House locks its decision after first attempt

        if (Random.value > 0.5f) // 50% chance
        {
            VoteCount++;
            Debug.Log($"{gameObject.name} influenced! Total Votes: {VoteCount}");

            PlaySound(successSound);
        }
        else
        {
            Debug.Log($"{gameObject.name} resisted influence.");

            PlaySound(failSound);
        }
    }

    private void ResetInfluenceTimer()
    {
        influenceDuration = Random.Range(minInfluenceTime, maxInfluenceTime);
        influenceTimer = influenceDuration;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasDecided)
        {
            playerNearby = true;
            ResetInfluenceTimer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}

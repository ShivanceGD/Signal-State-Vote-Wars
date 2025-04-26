using UnityEngine;

public class HouseManager : MonoBehaviour
{
    public GameObject ringsObject;
    private bool hasDecided = false;
    private bool playerNearby = false;

    public Material GreenMaterial;
    public Material RedMaterial;
    private float influenceTimer;
    private float influenceDuration;

    [SerializeField] private float minInfluenceTime = 2f;
    [SerializeField] private float maxInfluenceTime = 5f;

    public static int VoteCount = 0;

    [Header("Sounds")]
    public GameObject successSoundObject;  // Assign GameObject that has AudioSource with success clip
    public GameObject failSoundObject;     // Assign GameObject that has AudioSource with fail clip

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
            ringsObject.GetComponent<MeshRenderer>().material.color = Color.green;
            PlaySound(successSoundObject);
        }
        else
        {
            Debug.Log($"{gameObject.name} resisted influence.");
            ringsObject.GetComponent<MeshRenderer>().material.color = Color.red;
            PlaySound(failSoundObject);
        }
    }

    private void ResetInfluenceTimer()
    {
        influenceDuration = Random.Range(minInfluenceTime, maxInfluenceTime);
        influenceTimer = influenceDuration;
    }

    private void PlaySound(GameObject soundObject)
    {
        if (soundObject != null)
        {
            AudioSource source = soundObject.GetComponent<AudioSource>();
            if (source != null)
            {
                source.Play();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Influence") && !hasDecided)
        {
            playerNearby = true;
            ResetInfluenceTimer();
        }
        if (other.CompareTag("Influence"))
        {
            if (ringsObject != null)
                ringsObject.SetActive(true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Influence"))
        {
            playerNearby = false;
            if (ringsObject != null)
                ringsObject.SetActive(false);

            BoatController.canPlaySpeech = false;
        }

    }
}
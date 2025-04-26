using UnityEngine;

public class BoatController : MonoBehaviour
{
    public float speed = 10f;
    public float reverseSpeed = 5f;
    public float torque = 5f;
    private float driftFactor = 0.8f;

    private Rigidbody rb;

    // Speed upgrade
    public float speedIncrease = 5f;
    private float speedTimer = 0f;
    public float setSpeedTimer = 3f; // How long the upgrade lasts

    // Audio Sources
    public AudioSource boostSound;

    // Fuel System
    [Header("Fuel System")]
    public float maxFuel = 100f;
    public float currentFuel;
    public float fuelConsumptionPerUnit = 0.1f; // Fuel consumed per unit of distance
    private Vector3 lastPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 1f; // Adds drag to movement
        rb.angularDamping = 2f; // Adds drag to rotation

        // Initialize Fuel
        currentFuel = maxFuel;
        lastPosition = transform.position;
    }

    private void Update()
    {
        // Countdown speed timer
        if (speedTimer > 0)
        {
            speedTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        Move();
        UpdateFuel();
    }

    private void Move()
    {
        if (currentFuel <= 0)
            return; // No fuel, no movement

        // Determine current speed values based on upgrade
        float currentSpeed = (speedTimer > 0) ? speed + speedIncrease : speed;
        float currentReverseSpeed = (speedTimer > 0) ? reverseSpeed + speedIncrease : reverseSpeed;

        // Forward movement
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * currentSpeed);
        }

        // Reverse movement
        if (Input.GetKey(KeyCode.S))
        {
            if (speedTimer < 0)
            {
                rb.AddForce(-transform.forward * reverseSpeed);
            }
            else
            {
                rb.AddForce(transform.forward * (reverseSpeed + speedIncrease));
            }
        }

        // Steering based on speed
        float turnInput = Input.GetAxis("Horizontal");
        float t = Mathf.Lerp(0, torque, rb.linearVelocity.magnitude / 5f);
        rb.angularVelocity = transform.up * t * turnInput;

        // Apply drifting
        rb.linearVelocity = ForwardVelocity() + RightVelocity() * driftFactor;
    }

    private void UpdateFuel()
    {
        // Calculate distance traveled since last frame
        float distance = Vector3.Distance(transform.position, lastPosition);

        // Reduce fuel based on distance
        currentFuel -= distance * fuelConsumptionPerUnit;

        // Clamp fuel so it doesn't go below 0
        currentFuel = Mathf.Max(currentFuel, 0f);

        // Update last position
        lastPosition = transform.position;
    }

    Vector3 ForwardVelocity()
    {
        return transform.forward * Vector3.Dot(rb.linearVelocity, transform.forward);
    }

    Vector3 RightVelocity()
    {
        return transform.right * Vector3.Dot(rb.linearVelocity, transform.right);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Speed"))
        {
            // Add a one-time forward boost (impulse)
            rb.AddForce(transform.forward * (speed / 1.5f), ForceMode.Impulse);

            // Activate speed upgrade timer
            speedTimer = setSpeedTimer;

            PlaySoundOnSpeedBoost();
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Fuel"))
        {
            Refuel();
            Destroy(other.gameObject);
        }
    }

    void PlaySoundOnSpeedBoost()
    {
        if (boostSound != null)
        {
            boostSound.Play();
        }
        else
        {
            Debug.Log("No Sound Of Speed Boost");
        }
    }

    void Refuel()
    {
        currentFuel = maxFuel;
        Debug.Log("Fuel Refilled!");
    }
}
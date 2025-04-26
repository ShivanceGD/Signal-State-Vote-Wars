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

    //Audio Sources
    public AudioSource boostSound;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 1f; // Adds drag to movement
        rb.angularDamping = 2f; // Adds drag to rotation
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
    }

    private void Move()
    {
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
}
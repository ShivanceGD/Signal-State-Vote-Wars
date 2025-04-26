using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothing = 5.0f;

    Vector3 offset; //Distance between the player and the camera
    private Transform target; //Transform of the targer

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (!target)
        {
            Debug.Log("Error: Player not found for tracking script, have you tagged it correctly?");
        }
        offset = transform.position - target.position;
    }

    private void FixedUpdate()
    {
        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime); //Move smoothly between current posistion and target position at a max rate of smoothing
    }
}

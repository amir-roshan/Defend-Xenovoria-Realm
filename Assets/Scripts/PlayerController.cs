using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("General Setup Settings")]
    [Tooltip("How fast ship moves up and down based upon player input")][SerializeField] float controlSpeed = 10f;
    [Tooltip("Set boundaries based on the player's X axis")][SerializeField] float xRange = 10f;
    [Tooltip("Set boundaries based on the player's Y axis")][SerializeField] float yRange = 9f;
    [Tooltip("Add all of your lasers here")][SerializeField] GameObject[] lasers;

    [Header("Screen position based on tuning")]
    [SerializeField] float positionPitchFactor = -2f;
    [SerializeField] float positionYawFactor = -2f;

    [Header("Player input based on tuning")]
    [SerializeField] float controlPitchFactor = -10f;
    [SerializeField] float controlRollFactor = 20f;
    [SerializeField] float rotationFactor = .5f; // Adjust this based on how fast you want the rotation to occur

    [Header("Player Input")]
    [SerializeField] InputAction movement;
    [SerializeField] InputAction fire;

    float xThrow, yThrow;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnEnable() // it starts after awake and before start function 
    {
        movement.Enable();
        fire.Enable();
    }
    private void OnDisable()
    {
        movement.Disable();
        fire.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessTranslation();
        ProcessRotation();
        ProcessFiring();
    }

    void ProcessTranslation()
    {
        xThrow = movement.ReadValue<Vector2>().x;
        float xOffset = xThrow * Time.deltaTime * controlSpeed;
        float rawXpos = transform.localPosition.x + xOffset;
        float clampedXpos = Mathf.Clamp(rawXpos, -xRange, xRange);

        yThrow = movement.ReadValue<Vector2>().y;
        float yOffset = yThrow * Time.deltaTime * controlSpeed;
        float rawYpos = transform.localPosition.y + yOffset;
        float clampedYpos = Mathf.Clamp(rawYpos, -yRange, yRange); // Adding boundaries to our movement

        transform.localPosition = new Vector3(clampedXpos, clampedYpos, transform.localPosition.z);
    }
    void ProcessRotation()
    {
        float pitchDueToPosition = transform.localPosition.y * positionPitchFactor;
        float pitchDueToControlThrow = yThrow * controlPitchFactor;

        float yawDueToPosition = transform.localPosition.x * positionYawFactor;

        float rollDueToControlThrow = xThrow * controlRollFactor;

        float pitch = pitchDueToPosition + pitchDueToControlThrow;
        float yaw = yawDueToPosition;
        float roll = rollDueToControlThrow;

        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, roll);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotationFactor);
        //  returns a new rotation that gradually moves from the current rotation towards the target rotation.

    }
    void ProcessFiring()
    {
        if (fire.ReadValue<float>() > .5f)
        {
            SetLasersActive(true);
        }
        else
        {
            SetLasersActive(false);
        }
    }

    void SetLasersActive(bool isActive)
    {
        foreach (GameObject laser in lasers)
        {
            var laserParticleSystem = laser.GetComponent<ParticleSystem>().emission;
            laserParticleSystem.enabled = isActive; // Enable/Disable emission
        }

    }
}

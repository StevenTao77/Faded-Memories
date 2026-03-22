using UnityEngine;

public class FloatingBillboard : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatSpeed = 3f;
    public float floatHeight = 0.1f;

    private Transform mainCamera;
    private Vector3 startLocalPos;

    void Start()
    {
        // Find the main camera in the scene
        mainCamera = Camera.main.transform;

        // Record the initial starting position
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        // 1. Billboard Effect: Make the object always face the camera
        transform.LookAt(transform.position + mainCamera.forward);

        // 2. Floating Effect: Calculate the new Y position using a Sine wave
        float newY = startLocalPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // Apply the new position
        transform.localPosition = new Vector3(startLocalPos.x, newY, startLocalPos.z);
    }
}
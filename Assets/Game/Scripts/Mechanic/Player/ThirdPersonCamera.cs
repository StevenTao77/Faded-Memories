using UnityEngine;
using UnityEngine.InputSystem;
public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;        // The character to follow
    public float distance = 5f;     // Distance from character
    public float sensitivity = 3f;  // Mouse sensitivity
    public float yMin = -20f;       // Min vertical angle
    public float yMax = 60f;        // Max vertical angle
    public float height = 2f;       // Height offset

    private float yaw = 0f;
    private float pitch = 20f;

    void LateUpdate()
    {
        if (!target) return;

        // 1. Mouse Input
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, yMin, yMax);

        // 2. Calculate Rotation and Position
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        // 3. Apply to Camera
        transform.position = target.position + Vector3.up * height + offset;
        transform.LookAt(target.position + Vector3.up * height);
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;         
    public float distance = 5f;      
    public float sensitivity = 3f;   
    public float yMin = -20f;        
    public float yMax = 60f;         
    public float height = 2f;        

    private float yaw = 0f;
    private float pitch = 20f;

    void LateUpdate()
    {
        if (!target) return;
         
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, yMin, yMax);
         
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
         
        transform.position = target.position + Vector3.up * height + offset;
        transform.LookAt(target.position + Vector3.up * height);
    }
}
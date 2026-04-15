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
        mainCamera = Camera.main.transform;
         
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
         
        transform.LookAt(transform.position + mainCamera.forward);
         
        float newY = startLocalPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
         
        transform.localPosition = new Vector3(startLocalPos.x, newY, startLocalPos.z);
    }
}
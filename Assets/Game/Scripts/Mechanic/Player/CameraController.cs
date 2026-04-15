using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // Reference to the player object
     
    public enum CameraMode { TopDown, FreeAngle }
    public CameraMode currentMode = CameraMode.FreeAngle;

    [Header("Top-Down Settings ")]
    public Vector3 topDownOffset = new Vector3(0, 10, -10);  
    public float smoothSpeed = 5f;

    [Header("Free Angle Settings ")]
    public float sensitivityX = 2f;
    public float sensitivityY = 2f;
    public float distance = 5f; // Distance from the target
    public Vector2 pitchLimits = new Vector2(-10f, 60f);  

    private float currentX = 0f;
    private float currentY = 0f;

    void Start()
    { 
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        if (currentMode == CameraMode.FreeAngle)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()  
    {
        if (target == null) return;

        if (currentMode == CameraMode.TopDown)
        {
            HandleTopDownView();
        }
        else if (currentMode == CameraMode.FreeAngle)
        {
            HandleFreeAngleView();
        }
    }
     
    void HandleTopDownView()
    {
       
        Vector3 desiredPosition = target.position + topDownOffset;

         
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
         
        transform.LookAt(target);
    }
     
    void HandleFreeAngleView()
    { 
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            currentX += Input.GetAxis("Mouse X") * sensitivityX;
            currentY -= Input.GetAxis("Mouse Y") * sensitivityY;
             
            currentY = Mathf.Clamp(currentY, pitchLimits.x, pitchLimits.y);
        }   

         

        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // Calculate orbit position based on rotation and distance
        transform.position = target.position + rotation * dir;

        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
     
    public void SetMode(int index)
    {
        if (index == 0)
        {
            currentMode = CameraMode.TopDown;
        }
        else
        {
            currentMode = CameraMode.FreeAngle;
        }
    }
}
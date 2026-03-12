using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // Reference to the player object

    // Define available camera modes
    public enum CameraMode { TopDown, FreeAngle }
    public CameraMode currentMode = CameraMode.FreeAngle;

    [Header("Top-Down Settings (LoL Style)")]
    public Vector3 topDownOffset = new Vector3(0, 10, -10); // Offset for height and distance
    public float smoothSpeed = 5f;

    [Header("Free Angle Settings (PUBG Style)")]
    public float sensitivityX = 2f;
    public float sensitivityY = 2f;
    public float distance = 5f; // Distance from the target
    public Vector2 pitchLimits = new Vector2(-10f, 60f); // Vertical rotation limits

    private float currentX = 0f;
    private float currentY = 0f;

    void Start()
    {
        // Initialize rotation angles based on current transform
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        if (currentMode == CameraMode.FreeAngle)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate() // Use LateUpdate for camera following to prevent jitter
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

    // Logic for Top-Down (LoL style) view
    void HandleTopDownView()
    {
       
        Vector3 desiredPosition = target.position + topDownOffset;

         
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
         
        transform.LookAt(target);
    }

    // Logic for Free Angle (TPS style) view
    void HandleFreeAngleView()
    {
        // Only rotate camera if the cursor is locked (gameplay active)
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            currentX += Input.GetAxis("Mouse X") * sensitivityX;
            currentY -= Input.GetAxis("Mouse Y") * sensitivityY;

            // Clamp vertical rotation to prevent flipping
            currentY = Mathf.Clamp(currentY, pitchLimits.x, pitchLimits.y);
        }   

         

        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // Calculate orbit position based on rotation and distance
        transform.position = target.position + rotation * dir;

        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    // Public method to switch modes (called by UI)
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
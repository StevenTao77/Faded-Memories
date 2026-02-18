using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float sprintMultiplier = 2.0f; // New: Speed multiplier for sprinting
    [SerializeField] private float rotateSpeed = 10.0f;
    [SerializeField] private float jumpForce = 7.0f;

    [Header("References")]
    public CameraController camController;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool jumpInput;
    private bool isSprinting; // New: Track if shift is held

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetInput();
        HandleRotation();
    }

    private void FixedUpdate()
    {
        Move();

        if (jumpInput)
        {
            Jump();
            jumpInput = false;
        }
    }

    private void GetInput()
    {
        if (Keyboard.current == null) return;

        // --- 1. Movement Input (WASD) ---
        float x = 0;
        float z = 0;

        if (Keyboard.current.dKey.isPressed) x = 1;
        if (Keyboard.current.aKey.isPressed) x = -1;
        if (Keyboard.current.wKey.isPressed) z = 1;
        if (Keyboard.current.sKey.isPressed) z = -1;

        moveInput = new Vector2(x, z).normalized;

        // --- 2. Sprint Input (Left Shift) ---
        // Check if the key is currently being held down
        isSprinting = Keyboard.current.leftShiftKey.isPressed;

        // --- 3. Jump Input (Space) ---
        if (Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            jumpInput = true;
        }
    }

    private void Move()
    {
        Vector3 targetVelocity = Vector3.zero;

        // Calculate the current speed based on sprinting state
        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        if (camController != null && camController.currentMode == CameraController.CameraMode.FreeAngle)
        {
            // FreeAngle (TPS) Logic
            Transform camTransform = Camera.main.transform;

            Vector3 camForward = Vector3.Scale(camTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = Vector3.Scale(camTransform.right, new Vector3(1, 0, 1)).normalized;

            Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;

            // Apply currentSpeed instead of moveSpeed
            targetVelocity = moveDir * currentSpeed;
        }
        else
        {
            // TopDown (LoL) Logic
            targetVelocity = new Vector3(moveInput.x, 0, moveInput.y) * currentSpeed;
        }

        // Preserve gravity (Y velocity)
        targetVelocity.y = rb.linearVelocity.y;

        rb.linearVelocity = targetVelocity;
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void HandleRotation()
    {
        Vector3 lookDirection = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        if (lookDirection.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotateSpeed * Time.deltaTime);
        }
    }

    private bool IsGrounded()
    {
        float distToGround = GetComponent<Collider>().bounds.extents.y;
        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f);
    }
}
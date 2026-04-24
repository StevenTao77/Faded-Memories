using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotateSpeed = 10.0f;

    [Header("Model Correction")] 
    [SerializeField] private float modelXOffset = -90f;

    [Header("References")]
    public CameraController camController;

    [Header("Animation & Equimment States")]
    public GameObject torchReference; // Reference to the torch GameObject for animation purposes

    private Rigidbody rb;
    private Animator animator;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Grab the Animator from the player model
        animator = GetComponentInChildren<Animator>();

        // Double check to ensure physics won't tip the capsule over Edit: and so the gramps wont spin;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        Move();
        HandleRotation();
        UpdateAnimation();
    }

    private void GetInput()
    {
        if (Keyboard.current == null) return;

        float x = 0;
        float z = 0;

        if (Keyboard.current.dKey.isPressed) x = 1;
        if (Keyboard.current.aKey.isPressed) x = -1;
        if (Keyboard.current.wKey.isPressed) z = 1;
        if (Keyboard.current.sKey.isPressed) z = -1;

        moveInput = new Vector2(x, z).normalized;
    }

    private void Move()
    {
        Vector3 targetVelocity = Vector3.zero;

        if (camController != null && camController.currentMode == CameraController.CameraMode.FreeAngle)
        {
            Transform camTransform = Camera.main.transform;
            Vector3 camForward = Vector3.Scale(camTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = Vector3.Scale(camTransform.right, new Vector3(1, 0, 1)).normalized;

            Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;
            targetVelocity = moveDir * moveSpeed;
        }
        else
        {
            targetVelocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
        }

        targetVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVelocity;
    }

    private void HandleRotation()
    {
        Vector3 lookDirection = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        if (lookDirection.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDirection);

            
            Quaternion finalRotation = Quaternion.Euler(modelXOffset, targetRot.eulerAngles.y, 0);
             
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, finalRotation, rotateSpeed * Time.fixedDeltaTime));
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;
         
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        // Pass the speed to the Animator parameter
        animator.SetFloat("Speed", currentSpeed);

        if (torchReference != null)
        {
            bool isTorchVisible = torchReference.activeInHierarchy;
            animator.SetBool("IsHoldingTorch", isTorchVisible);
        }
    }
}
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DialogueTrigger : MonoBehaviour
{
    [Header("Narritive File")]
 
    public TextAsset inkAsset;

    [Header("UI Settings (Optional)")]
    
    public GameObject interactPrompt;

    private bool playerInRange = false;
    private DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = FindFirstObjectByType<DialogueManager>();

        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager is missing from the scene!");
        }

        GetComponent<Collider>().isTrigger = true;

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.G))
        {
            if (dialogueManager != null && !dialogueManager.dialoguePanel.activeInHierarchy && inkAsset != null)
            {
                if (interactPrompt != null) interactPrompt.SetActive(false);
                dialogueManager.StartDialogue(inkAsset);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactPrompt != null && !dialogueManager.dialoguePanel.activeInHierarchy)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Auto Fit To Parent")]
    private void AutoFitColliderToParent()
    {
        if (transform.parent == null)
        {
            Debug.LogWarning("This Prefab is not a child of any object!");
            return;
        }

        BoxCollider myCollider = GetComponent<BoxCollider>();
        if (myCollider == null) return;

        //  Find the parent's Renderer to measure its visual size
        Renderer parentRenderer = transform.parent.GetComponentInChildren<Renderer>();

        if (parentRenderer != null)
        {
            // Reset to prevent cumulative offset
            transform.localPosition = Vector3.zero;
            myCollider.center = Vector3.zero;

            // Get the bounding box of the parent object
            Bounds parentBounds = parentRenderer.bounds;

            // Calculate the total height of the generated collider (1.5x of parent height)
            float totalGeneratedHeight_Y = parentBounds.size.y * 1.5f;

            // Set the BoxCollider Size (handles arbitrary scaling on the parent)
            myCollider.size = new Vector3(
                (parentBounds.size.x / transform.lossyScale.x) * 2f,
                (totalGeneratedHeight_Y / transform.lossyScale.y),
                (parentBounds.size.z / transform.lossyScale.z) * 2f
            );

            // THE MAGIC: Align the BOTTOM of the collider to the BOTTOM of the object
            // Calculate how far the object's origin is from its "feet". 
            // Handle negative if origin is below feet, positive if above (common).
            float offset_ToFeet = transform.parent.position.y - parentBounds.min.y;

            // Position the prefab so its generated collider's bottom edge is at parent's feet.
            // Formula: Required_Y_Pos = GeneratedHeight_Y / 2 - Offset_Origin_To_Feet
            float newLocal_Y_Pos = (totalGeneratedHeight_Y / 2.0f) - offset_ToFeet;

            // Divide by parent's local scale to ensure it is in local units
            transform.localPosition = new Vector3(0, newLocal_Y_Pos / transform.parent.lossyScale.y, 0);

            // Keep the center 0,0,0 within the prefab
            myCollider.center = Vector3.zero;

            Debug.Log("Success: Trigger box auto-sized to 2x and positioned to sit on the object's base!");
        }
        else
        {
            Debug.LogWarning("Parent has no Renderer. Cannot auto-calculate size.");
        }
    }
#endif
}
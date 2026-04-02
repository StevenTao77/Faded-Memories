using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DialogueTrigger : MonoBehaviour
{
    [Header("Narrative File")]
    public TextAsset inkAsset;

    [Header("UI Settings (Optional)")]
    public GameObject interactPrompt;

    private bool playerInRange = false;

    // We no longer need to find the DialogueManager in Start.
    // We will just call DialogueManager.Instance when needed!

    private void Start()
    {
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
            // Make sure the DialogueManager and UIManager exist
            if (DialogueManager.Instance != null && UIManager.Instance != null)
            {
                // Check if the dialogue panel in UIManager is NOT active, and we have an ink file
                if (UIManager.Instance.dialoguePanel != null &&
                    !UIManager.Instance.dialoguePanel.activeInHierarchy &&
                    inkAsset != null)
                {
                    if (interactPrompt != null) interactPrompt.SetActive(false);

                    // Tell the DialogueManager to start the story
                    DialogueManager.Instance.StartDialogue(inkAsset);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // Only show prompt if the dialogue panel is NOT currently open
            if (interactPrompt != null && UIManager.Instance != null &&
                UIManager.Instance.dialoguePanel != null &&
                !UIManager.Instance.dialoguePanel.activeInHierarchy)
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
            float offset_ToFeet = transform.parent.position.y - parentBounds.min.y;

            // Position the prefab so its generated collider's bottom edge is at parent's feet.
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
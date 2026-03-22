using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent

public class UniversalInteractable : MonoBehaviour
{
    [Header("UI Prompt")]
    [Tooltip("Drag the 3D Text or Sprite (EPrompt) here")]
    public GameObject ePromptVisual;

    [Header("Interaction Action")]
    [Tooltip("What happens when the player presses E? Configure this in the Inspector!")]
    public UnityEvent onInteract;

    private bool canInteract = false;

    private void Start()
    {
        if (ePromptVisual != null)
        {
            ePromptVisual.SetActive(false);
        }
    }

    private void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            // This will execute whatever you set up in the Inspector!
            onInteract.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            if (ePromptVisual != null)
            {
                ePromptVisual.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            if (ePromptVisual != null)
            {
                ePromptVisual.SetActive(false);
            }
        }
    }
}
using UnityEngine;
using UnityEngine.Events;  

public class UniversalInteractable : MonoBehaviour
{
    [Header("UI Prompt")]
    
    public GameObject ePromptVisual;

    [Header("Interaction Action")]
    
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
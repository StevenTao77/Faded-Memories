using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    
     
    private IInteractable currentInteractable;

    private void Update()
    { 
        if (currentInteractable != null && Input.GetKeyDown(currentInteractable.InteractKey))
        {
            //polymorphic call to the Interact method of the current interactable object
            currentInteractable.Interact();

            if (currentInteractable != null)
            {
                currentInteractable.TogglePrompt(false);
            }

            currentInteractable = null;
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
         
        IInteractable interactable = other.GetComponent<IInteractable>();

        if (interactable != null)
        {
            currentInteractable = interactable;
            currentInteractable.TogglePrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
         
        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable.TogglePrompt(false);
            currentInteractable = null;
        }
    }
}
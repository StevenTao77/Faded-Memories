using UnityEngine;
using UnityEngine.Events;

public class DoorTriggerController : MonoBehaviour , IInteractable
{
    
    public Animator doorAnimator;
    public GameObject promptVisual;

    private bool isOpen = false;
   

    public UnityEvent onInteract;

    public KeyCode InteractKey => KeyCode.E;


    private void Start()
    {

        if (promptVisual != null) promptVisual.SetActive(false);
    }
    public void Interact()
    {
        
        onInteract?.Invoke();
    }

    public void TogglePrompt(bool show)
    {
        if (promptVisual != null)
        {
            promptVisual.SetActive(show);
        }

        isOpen = !isOpen;

        if (doorAnimator != null)
        {
            doorAnimator.SetBool("isOpen", show);
        }
    }
 
}
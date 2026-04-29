using UnityEngine;
using UnityEngine.Events;

public class DoorTriggerController : BaseInteractable
{
    public Animator doorAnimator;
    private bool isOpen = false;

    public UnityEvent onInteract;

    public override void Interact()
    {
        onInteract?.Invoke();
    }

    public override void TogglePrompt(bool show)
    {
        base.TogglePrompt(show);  

        isOpen = !isOpen;
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("isOpen", show);
        }
    }
}
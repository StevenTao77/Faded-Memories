using UnityEngine;

public abstract class BaseInteractable : MonoBehaviour, IInteractable
{
    [Header("UI Prompt")]
    public GameObject promptVisual;

    public virtual KeyCode InteractKey => KeyCode.E;

    protected virtual void Start()
    {
        if (promptVisual != null) promptVisual.SetActive(false);
    }

    public abstract void Interact();

    public virtual void TogglePrompt(bool show)
    {
        if (promptVisual != null) promptVisual.SetActive(show);
    }
}
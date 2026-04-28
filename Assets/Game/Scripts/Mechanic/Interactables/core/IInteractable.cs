
using UnityEngine;

public interface IInteractable
{
    KeyCode InteractKey { get; }
    //define the interaction behavior
    void Interact();
    //show or hide the interaction prompt
    void TogglePrompt(bool show);
}
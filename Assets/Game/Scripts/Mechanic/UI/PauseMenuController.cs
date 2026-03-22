using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    // The settingsPanel variable is completely DELETED!
    // No more dragging needed in the Inspector.

    private bool isPaused = false;

    void Update()
    {
        // Check if ESC is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        // 1. Tell the UIManager to show the settings panel
        UIManager.Instance.ToggleSettingsPanel(true);

        // 2. Freeze time (game stops moving)
        Time.timeScale = 0f;

        // 3. Unlock the cursor so you can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    public void ResumeGame()
    {
        // 1. Tell the UIManager to hide the settings panel
        UIManager.Instance.ToggleSettingsPanel(false);

        // 2. Unfreeze time
        Time.timeScale = 1f;

        // 3. Lock cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
    }
}
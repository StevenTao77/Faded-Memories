using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    // Drag your "Option/setting" GameObject here in the Inspector
    public GameObject settingsPanel;

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
        // Show the settings panel
        settingsPanel.SetActive(true);

        // Freeze time (game stops moving)
        Time.timeScale = 0f;

        // Unlock the cursor so you can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    public void ResumeGame()
    {
        // Hide the settings panel
        settingsPanel.SetActive(false);

        // Unfreeze time
        Time.timeScale = 1f;

        // Lock cursor again (if your game is an FPS/TPS)
        // If your game is a point-and-click game, remove the next two lines
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
    }
}
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    // The settingsPanel variable is completely DELETED!
    // No more dragging needed in the Inspector.
    
    [SerializeField] private GameObject gameplayCanvas;

    private bool isPaused = false;
    public static PauseMenuController Instance { get; private set; }

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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

        // 2. Disable the gameplay canvas
        if (gameplayCanvas != null)
        {
            gameplayCanvas.SetActive(false);
        }

        // 3. Freeze time (game stops moving)
        Time.timeScale = 0f;

        // 4. Unlock the cursor so you can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    public void ResumeGame()
    {
        // 1. Tell the UIManager to hide the settings panel
        UIManager.Instance.ToggleSettingsPanel(false);

        // 2. Enable the gameplay canvas
        if (gameplayCanvas != null)
        {
            gameplayCanvas.SetActive(true);
        }

        // 3. Unfreeze time
        Time.timeScale = 1f;

        // 4. Lock cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
    }
}
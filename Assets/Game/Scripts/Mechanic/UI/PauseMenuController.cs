using UnityEngine;

public class PauseMenuController : MonoBehaviour
{ 
    
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
        UIManager.Instance.ToggleSettingsPanel(true);
         
        if (gameplayCanvas != null)
        {
            gameplayCanvas.SetActive(false);
        }
         
        Time.timeScale = 0f;
         
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    public void ResumeGame()
    {
         
        UIManager.Instance.ToggleSettingsPanel(false);
         
        if (gameplayCanvas != null)
        {
            gameplayCanvas.SetActive(true);
        }
         
        Time.timeScale = 1f;
         
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
    }
}
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject gameplayCanvas;

    private bool isPaused = false;
    public static PauseMenuController Instance { get; private set; }

    // Use Awake for Singleton initialization
    private void Awake()
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

        // if reference is lost after scene switch, find it dynamically
        ValidateCanvasReference();

        if (gameplayCanvas != null)
        {
            gameplayCanvas.SetActive(false);
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    //public void ResumeGame()
    //{
    //    UIManager.Instance.ToggleSettingsPanel(false);

      
    //    ValidateCanvasReference();

    //    if (gameplayCanvas != null)
    //    {
    //        gameplayCanvas.SetActive(true);
    //    }

    //    Time.timeScale = 1f;

    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;

    //    isPaused = false;
    //}

    public void ResumeGame()
    {
        UIManager.Instance.ToggleSettingsPanel(false);

        ValidateCanvasReference();

        if (gameplayCanvas != null)
        {
            
            bool isCinematicActive = GlobalCinematicManager.Instance != null && GlobalCinematicManager.Instance.IsPlaying;

            
            if (!isCinematicActive)
            {
                gameplayCanvas.SetActive(true);
            }
        }

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
    }
     
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
     
    private void ValidateCanvasReference()
    {
        if (gameplayCanvas == null)
        { 
            GameObject foundCanvas = GameObject.Find("Gameplay Canvas");

            if (foundCanvas != null)
            {
                gameplayCanvas = foundCanvas;
            }
            else
            {
                Debug.Log("PauseMenuController: Gameplay Canvas not found in the current scene!");
            }
        }
    }
}
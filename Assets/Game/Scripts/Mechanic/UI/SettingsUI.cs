using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsUI : MonoBehaviour
{
    [Header("Mechanic References")]
    public CameraController camController;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        BindApplyButton();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BindApplyButton();
    }

    private void BindApplyButton()
    {
        if (UIManager.Instance != null && UIManager.Instance.applyButton != null)
        {
            UIManager.Instance.applyButton.onClick.RemoveListener(OnApply);
            UIManager.Instance.applyButton.onClick.AddListener(OnApply);
        }
        else
        {
            Debug.LogWarning("SettingsUI: UIManager or Apply Button is missing during binding!");
        }
    }

    void OnApply()
    {
        if (UIManager.Instance == null || UIManager.Instance.viewDropdown == null) return;

        int value = UIManager.Instance.viewDropdown.value;

        
        if (camController == null)
        {
            camController = FindObjectOfType<CameraController>();

            if (camController == null)
            {
                Debug.LogWarning("SettingsUI: Could not find any CameraController in the current scene!");
            }
        }

        // Apply camera mode if we successfully found the controller
        if (camController != null)
        {
            camController.SetMode(value);
        }

        UIManager.Instance.ToggleSettingsPanel(false);

        Time.timeScale = 1f;

        if (value == 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    [Header("Mechanic References")]
    public CameraController camController;

    void Start()
    {
        // Tell the apply button inside the UIManager to run the OnApply method
        if (UIManager.Instance != null && UIManager.Instance.applyButton != null)
        {
            UIManager.Instance.applyButton.onClick.AddListener(OnApply);
        }
        else
        {
            Debug.LogWarning("UIManager or Apply Button is not set up correctly!");
        }
    }

    void OnApply()
    {
        // 1. Get the dropdown value directly from the UIManager
        int value = UIManager.Instance.viewDropdown.value;

        // 2. Update Camera Mode
        camController.SetMode(value);

        // 3. Tell the UIManager to close the settings panel
        UIManager.Instance.ToggleSettingsPanel(false);

        // 4. Resume game flow
        Time.timeScale = 1f;

        // 5. Handle Cursor State based on mode
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
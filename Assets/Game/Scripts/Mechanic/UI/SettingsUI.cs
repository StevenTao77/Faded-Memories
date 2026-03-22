using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [Header("References")]
    public CameraController camController;
    public TMP_Dropdown viewDropdown;
    public Button applyButton;
    public GameObject settingsPanel;

    void Start()
    {
        applyButton.onClick.AddListener(OnApply);
    }

    void OnApply()
    {
        // Get selection and update Camera Mode (0 = TopDown, 1 = FreeAngle)
        int value = viewDropdown.value;
        camController.SetMode(value);

        // Resume game flow and close the menu
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;

        // Handle Cursor State based on mode
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
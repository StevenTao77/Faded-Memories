using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    
    public static UIManager Instance { get; private set; }

    [Header("--- Settings & Pause Menu UI ---")]
    [Tooltip("The main panel for settings (used by PauseMenu and SettingsUI)")]
    public GameObject settingsPanel;

    [Tooltip("The dropdown for camera view selection")]
    public TMP_Dropdown viewDropdown;

    [Tooltip("The apply button in the settings menu")]
    public Button applyButton;

    [Header("--- Dialogue UI ---")]
    [Tooltip("The main panel for conversations")]
    public GameObject dialoguePanel;

    [Tooltip("The text component that displays the dialogue")]
    public TextMeshProUGUI dialogueText;

    [Tooltip("The container transform where choice buttons will spawn")]
    public Transform choiceButtonContainer;
     
    [Tooltip("The button prefab for dialogue choices")]
    public GameObject choiceButtonPrefab;

    private void Awake()
    {
         
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
     

    public void ToggleSettingsPanel(bool isOpen)
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(isOpen);
        }
    }

    public void ToggleDialoguePanel(bool isOpen)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(isOpen);
        }
    }
}
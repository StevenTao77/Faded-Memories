using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    
    public static UIManager Instance { get; private set; }

    [Header("--- Settings & Pause Menu UI ---")] 
    public GameObject settingsPanel;

    
    public TMP_Dropdown viewDropdown;

   
    public Button applyButton;

    [Header("--- Dialogue UI ---")] 
    public GameObject dialoguePanel;

     
    public TextMeshProUGUI dialogueText;
     
    public TextMeshProUGUI dialogueNameText;
     
    public Transform choiceButtonContainer;
      
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
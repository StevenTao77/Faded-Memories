using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Typewriter Effect")]
    public float typingSpeed = 0.02f;

    private Story currentStory;
    private Coroutine displayLineCoroutine;

    // Store reference to the trigger that started the current dialogue
    private DialogueTrigger currentActiveTrigger;

    private bool isDialogueActive = false;
    private bool isTyping = false;
    private string currentLineText = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ForceEndDialogue();
    }

    private void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleDialoguePanel(false);
        }
    }

    private void Update()
    {
        if (!isDialogueActive) return;

        if (UIManager.Instance == null)
        {
            ForceEndDialogue();
            return;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (currentStory.currentChoices.Count > 0 && !isTyping) return;

            if (isTyping)
            {
                StopCoroutine(displayLineCoroutine);
                if (UIManager.Instance != null && UIManager.Instance.dialogueText != null)
                {
                    UIManager.Instance.dialogueText.text = currentLineText;
                }
                isTyping = false;
                DisplayChoices();
            }
            else
            {
                ContinueDialogue();
            }
        }
    }

    // --- MODIFIED: Now requires the initiator trigger ---
    public void StartDialogue(TextAsset newInkAsset, DialogueTrigger initiator)
    {
        currentStory = new Story(newInkAsset.text);
        isDialogueActive = true;

        currentActiveTrigger = initiator;
        if (currentActiveTrigger != null)
        {
            currentActiveTrigger.ResetLineCounter();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleDialoguePanel(true);
        }

        SetPlayerControl(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ContinueDialogue();
    }

    private void ContinueDialogue()
    {
        ClearUI();

        if (currentStory.canContinue)
        {
            currentLineText = currentStory.Continue().Trim();
            SetNameInDialogue();

            // Tell the trigger to play its local voice line
            if (currentActiveTrigger != null)
            {
                currentActiveTrigger.PlayNextVoiceLine();
            }

            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }

            displayLineCoroutine = StartCoroutine(TypeSentence(currentLineText));
        }
        else if (currentStory.currentChoices.Count == 0)
        {
            EndDialogue();
        }
        else
        {
            DisplayChoices();
        }
    }

    private void SetNameInDialogue()
    {
        if (UIManager.Instance != null && UIManager.Instance.dialogueNameText != null)
        {
            if (currentStory.currentTags.Count > 0)
            {
                string nameTag = currentStory.currentTags[0];
                UIManager.Instance.dialogueNameText.text = nameTag;
            }
            else
            {
                UIManager.Instance.dialogueNameText.text = "";
            }
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        if (UIManager.Instance != null && UIManager.Instance.dialogueText != null)
        {
            UIManager.Instance.dialogueText.text = "";

            foreach (char letter in sentence.ToCharArray())
            {
                UIManager.Instance.dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        isTyping = false;
        DisplayChoices();
    }

    private void DisplayChoices()
    {
        if (UIManager.Instance == null) return;

        if (currentStory.currentChoices.Count > 0)
        {
            Transform container = UIManager.Instance.choiceButtonContainer;
            GameObject prefab = UIManager.Instance.choiceButtonPrefab;

            foreach (Choice choice in currentStory.currentChoices)
            {
                GameObject btnObj = Instantiate(prefab, container);
                Button button = btnObj.GetComponent<Button>();
                TextMeshProUGUI buttonText = btnObj.GetComponentInChildren<TextMeshProUGUI>();

                buttonText.text = choice.text;
                button.onClick.AddListener(() => OnClickChoice(choice));
            }
        }
    }

    private void ClearUI()
    {
        if (UIManager.Instance == null || UIManager.Instance.choiceButtonContainer == null) return;

        foreach (Transform child in UIManager.Instance.choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnClickChoice(Choice choice)
    {
        currentStory.ChooseChoiceIndex(choice.index);
        ContinueDialogue();
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        currentActiveTrigger = null;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleDialoguePanel(false);
        }

        SetPlayerControl(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ForceEndDialogue()
    {
        isDialogueActive = false;
        isTyping = false;
        currentActiveTrigger = null;

        if (displayLineCoroutine != null)
        {
            StopCoroutine(displayLineCoroutine);
            displayLineCoroutine = null;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleDialoguePanel(false);
            ClearUI();
        }
    }

    private void SetPlayerControl(bool canMove)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            MonoBehaviour moveScript = player.GetComponent("PlayerMovement") as MonoBehaviour;
            if (moveScript != null) moveScript.enabled = canMove;
        }

        if (Camera.main != null)
        {
            MonoBehaviour camScript = Camera.main.GetComponent("CameraController") as MonoBehaviour;
            if (camScript == null)
            {
                camScript = Camera.main.GetComponent("ThirdPersonCamera") as MonoBehaviour;
            }

            if (camScript != null) camScript.enabled = canMove;
        }
    }
}
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

    // --- SAFETY FEATURE 1: Scene Load Listener ---
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
        // Whenever a new scene loads, forcibly reset the dialogue state.
        // This prevents the ghost click bug completely.
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

        // --- SAFETY FEATURE 2: UI Existence Check ---
        // If the UI was destroyed (e.g., scene unloaded) but this manager survived,
        // kill the dialogue state immediately so clicks don't trigger errors.
        if (UIManager.Instance == null)
        {
            ForceEndDialogue();
            return;
        }

        // Normal input detection
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

    public void StartDialogue(TextAsset newInkAsset)
    {
        currentStory = new Story(newInkAsset.text);
        isDialogueActive = true;

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

    // Normal end dialogue when story finishes
    private void EndDialogue()
    {
        isDialogueActive = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleDialoguePanel(false);
        }

        SetPlayerControl(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // --- SAFETY FEATURE 3: Hard Reset ---
    // A clean wipe function that stops everything without assuming the UI still exists
    public void ForceEndDialogue()
    {
        isDialogueActive = false;
        isTyping = false;

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
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Transform choiceButtonContainer;
    public Button choiceButtonPrefab;

    [Header("Typewriter Effect")]
    public float typingSpeed = 0.02f;

    private Story currentStory;
    private Coroutine displayLineCoroutine;

    private void Awake()
    {
        // 1. Singleton pattern and protect this manager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 2. CRITICAL: Protect the Dialogue Canvas from being destroyed on scene load
            if (dialoguePanel != null)
            {
                Canvas parentCanvas = dialoguePanel.GetComponentInParent<Canvas>();
                if (parentCanvas != null)
                {
                    DontDestroyOnLoad(parentCanvas.gameObject);
                }
            }
        }
        else
        {
            if (dialoguePanel != null)
            {
                Canvas parentCanvas = dialoguePanel.GetComponentInParent<Canvas>();
                if (parentCanvas != null)
                {
                    Destroy(parentCanvas.gameObject);
                }
            }
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public void StartDialogue(TextAsset newInkAsset)
    {
        currentStory = new Story(newInkAsset.text);

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        // Lock player via dynamic search
        SetPlayerControl(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        RefreshUI();
    }

    private void RefreshUI()
    {
        ClearUI();

        string text = "";
        while (currentStory.canContinue)
        {
            text += currentStory.Continue();
        }

        if (displayLineCoroutine != null)
        {
            StopCoroutine(displayLineCoroutine);
        }

        displayLineCoroutine = StartCoroutine(TypeSentence(text.Trim()));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        DisplayChoices();
    }

    private void DisplayChoices()
    {
        if (currentStory.currentChoices.Count == 0)
        {
            Button button = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "End Dialogue";
            button.onClick.AddListener(() => EndDialogue());
            return;
        }

        foreach (Choice choice in currentStory.currentChoices)
        {
            Button button = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = choice.text;
            button.onClick.AddListener(() => OnClickChoice(choice));
        }
    }

    private void ClearUI()
    {
        foreach (Transform child in choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnClickChoice(Choice choice)
    {
        currentStory.ChooseChoiceIndex(choice.index);
        RefreshUI();
    }

    private void EndDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // Unlock player
        SetPlayerControl(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 3. Dynamically find the Player and Camera in the current scene
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
            // Tries to find either of your camera scripts
            MonoBehaviour camScript = Camera.main.GetComponent("CameraController") as MonoBehaviour;
            if (camScript == null)
            {
                camScript = Camera.main.GetComponent("ThirdPersonCamera") as MonoBehaviour;
            }

            if (camScript != null) camScript.enabled = canMove;
        }
    }
}
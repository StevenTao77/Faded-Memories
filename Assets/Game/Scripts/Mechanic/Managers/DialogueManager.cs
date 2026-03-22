using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    // The DialogueManager remains a Singleton for logic calls, 
    // but it no longer stores ANY direct UI references!
    public static DialogueManager Instance;

    [Header("Typewriter Effect")]
    public float typingSpeed = 0.02f;

    private Story currentStory;
    private Coroutine displayLineCoroutine;

    private void Awake()
    {
        // 1. Simplified Singleton pattern
        // We removed the massive Canvas protection code because the UI is now safely handled 
        // by the Island_UI scene and the UIManager.
        if (Instance == null)
        {
            Instance = this;
            // Optional: Keep it alive if you change scenes, but usually handled by additive loading now.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleDialoguePanel(false);
        }
    }

    public void StartDialogue(TextAsset newInkAsset)
    {
        currentStory = new Story(newInkAsset.text);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleDialoguePanel(true);
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
        if (UIManager.Instance != null && UIManager.Instance.dialogueText != null)
        {
            UIManager.Instance.dialogueText.text = "";

            foreach (char letter in sentence.ToCharArray())
            {
                UIManager.Instance.dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        DisplayChoices();
    }

    private void DisplayChoices()
    {
        if (UIManager.Instance == null) return;

        Transform container = UIManager.Instance.choiceButtonContainer;
        GameObject prefab = UIManager.Instance.choiceButtonPrefab;

        if (currentStory.currentChoices.Count == 0)
        {
            GameObject btnObj = Instantiate(prefab, container);
            Button button = btnObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = btnObj.GetComponentInChildren<TextMeshProUGUI>();

            buttonText.text = "End Dialogue";
            button.onClick.AddListener(() => EndDialogue());
            return;
        }

        foreach (Choice choice in currentStory.currentChoices)
        {
            GameObject btnObj = Instantiate(prefab, container);
            Button button = btnObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = btnObj.GetComponentInChildren<TextMeshProUGUI>();

            buttonText.text = choice.text;
            button.onClick.AddListener(() => OnClickChoice(choice));
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
        RefreshUI();
    }

    private void EndDialogue()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleDialoguePanel(false);
        }

        // Unlock player
        SetPlayerControl(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 3. Dynamically find the Player and Camera in the current scene (Unchanged)
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
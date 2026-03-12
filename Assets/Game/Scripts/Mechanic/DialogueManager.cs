using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;
using System.Collections; // NEEDED FOR COROUTINES

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextAsset inkAsset;
    public TextMeshProUGUI dialogueText;
    public Transform choiceButtonContainer;
    public Button choiceButtonPrefab;

    [Header("Player Settings")]
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour cameraScript;

    [Header("Typewriter Effect")]
    public float typingSpeed = 0.02f; // Speed of the typing effect

    private Story currentStory;
    private Coroutine displayLineCoroutine;

    private void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue()
    {
        currentStory = new Story(inkAsset.text);
        dialoguePanel.SetActive(true);

        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (cameraScript != null) cameraScript.enabled = false;

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

        // Stop the previous typing effect if it's still running
        if (displayLineCoroutine != null)
        {
            StopCoroutine(displayLineCoroutine);
        }

        // Start typing the new text
        displayLineCoroutine = StartCoroutine(TypeSentence(text.Trim()));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = ""; // Clear current text

        // Type each character one by one
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Only display choices AFTER the text has finished typing
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
        dialoguePanel.SetActive(false);

        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (cameraScript != null) cameraScript.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
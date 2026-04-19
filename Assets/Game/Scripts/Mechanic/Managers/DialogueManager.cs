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

             
            string speakerName = "";
            string videoToPlay = "";

            foreach (string tag in currentStory.currentTags)
            {
                if (tag.ToLower().StartsWith("video:"))
                {
                    videoToPlay = tag.Substring(6).Trim();  
                }
                else
                {
                    speakerName = tag;  
                }
            }

             
            if (UIManager.Instance != null && UIManager.Instance.dialogueNameText != null)
            {
                UIManager.Instance.dialogueNameText.text = speakerName;
            }

            if (currentActiveTrigger != null)
            {
                currentActiveTrigger.PlayNextVoiceLine();
            }

            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }

             
            if (!string.IsNullOrEmpty(videoToPlay) && currentActiveTrigger != null)
            {
                UnityEngine.Video.VideoClip clip = currentActiveTrigger.GetVideoByTag(videoToPlay);
                if (clip != null && GlobalCinematicManager.Instance != null)
                {
                    PlayCinematicMidDialogue(clip);
                }
                else
                {
                     
                    displayLineCoroutine = StartCoroutine(TypeSentence(currentLineText));
                }
            }
            else
            {
                 
                displayLineCoroutine = StartCoroutine(TypeSentence(currentLineText));
            }
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

     
    private void PlayCinematicMidDialogue(UnityEngine.Video.VideoClip clip)
    {
         
        if (UIManager.Instance != null) UIManager.Instance.ToggleDialoguePanel(false);

         
        GlobalCinematicManager.Instance.PlayCinematic(clip, () =>
        {
            
            if (UIManager.Instance != null) UIManager.Instance.ToggleDialoguePanel(true);
            displayLineCoroutine = StartCoroutine(TypeSentence(currentLineText));
        });
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
        if (currentActiveTrigger != null)
        {
            if (!string.IsNullOrEmpty(currentActiveTrigger.symbolName))
            {
                if (MemorySymbolManager.Instance != null)
                {
                    MemorySymbolManager.Instance.OnSymbolInteracted(currentActiveTrigger.symbolName);
                }
                else
                {
                    Debug.LogError("[DialogueManager] can't find MemorySymbolManager.Instance£¡");
                }
            }
        }

        string lastTriggerName = currentActiveTrigger != null ? currentActiveTrigger.symbolName.ToLower() : "";

        isDialogueActive = false;
        currentActiveTrigger = null;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleDialoguePanel(false);
        }

        if (lastTriggerName == "boat")
        {
            SceneManager.LoadScene("MainMenu_UI");
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

    public void SetPlayerControl(bool canMove)
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
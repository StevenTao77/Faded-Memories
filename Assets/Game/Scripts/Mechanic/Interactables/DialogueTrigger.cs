using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DialogueTrigger : MonoBehaviour
{
    [Header("Narrative Settings")]
    public TextAsset inkAsset;

    [Tooltip("Optional: Assign a video to play before this dialogue. Leave empty to skip video.")]
    public UnityEngine.Video.VideoClip TriggerVideo;

    [Header("UI Settings (Optional)")]
    public GameObject interactPrompt;

    [Header("Voice Settings (Local)")]
    public AudioClip voiceClip1;
    public AudioClip voiceClip2;
    public AudioClip voiceClip3;
    public AudioClip voiceClip4;
    public AudioSource voiceAudioSource;
    public bool playVoiceLines = true;

    private bool playerInRange = false;
    private int currentDialogueLine = 0;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        if (playVoiceLines && voiceAudioSource == null)
        {
            voiceAudioSource = GetComponent<AudioSource>();
            if (voiceAudioSource == null)
            {
                voiceAudioSource = gameObject.AddComponent<AudioSource>();
                voiceAudioSource.playOnAwake = false;
            }
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.G))
        {
            if (DialogueManager.Instance != null && UIManager.Instance != null)
            {
                if (UIManager.Instance.dialoguePanel != null &&
                    !UIManager.Instance.dialoguePanel.activeInHierarchy &&
                    inkAsset != null)
                {
                    if (interactPrompt != null) interactPrompt.SetActive(false);

                    // Route A: Play cinematic first, then dialogue
                    if (TriggerVideo != null && GlobalCinematicManager.Instance != null)
                    {
                        DialogueManager.Instance.SetPlayerControl(false);

                        GlobalCinematicManager.Instance.PlayCinematic(TriggerVideo, () =>
                        {
                            DialogueManager.Instance.StartDialogue(inkAsset, this);
                        });
                    }
                    // Route B: Start dialogue directly
                    else
                    {
                        DialogueManager.Instance.StartDialogue(inkAsset, this);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactPrompt != null && UIManager.Instance != null &&
                UIManager.Instance.dialoguePanel != null &&
                !UIManager.Instance.dialoguePanel.activeInHierarchy)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }

    public void ResetLineCounter()
    {
        currentDialogueLine = 0;
    }

    public void PlayNextVoiceLine()
    {
        if (!playVoiceLines || voiceAudioSource == null) return;

        AudioClip voiceClip = GetVoiceClipForCurrentLine();
        if (voiceClip != null)
        {
            voiceAudioSource.Stop();
            voiceAudioSource.clip = voiceClip;
            voiceAudioSource.Play();
        }

        currentDialogueLine++;
    }

    private AudioClip GetVoiceClipForCurrentLine()
    {
        return currentDialogueLine switch
        {
            0 => voiceClip1,
            1 => voiceClip2,
            2 => voiceClip3,
            3 => voiceClip4,
            _ => null
        };
    }

#if UNITY_EDITOR
    [ContextMenu("Auto Fit To Parent")]
    private void AutoFitColliderToParent()
    {
        if (transform.parent == null)
        {
            Debug.LogWarning("This Prefab is not a child of any object!");
            return;
        }

        BoxCollider myCollider = GetComponent<BoxCollider>();
        if (myCollider == null) return;

        Renderer parentRenderer = transform.parent.GetComponentInChildren<Renderer>();

        if (parentRenderer != null)
        {
            transform.localPosition = Vector3.zero;
            myCollider.center = Vector3.zero;

            Bounds parentBounds = parentRenderer.bounds;
            float totalGeneratedHeight_Y = parentBounds.size.y * 1.5f;

            myCollider.size = new Vector3(
                (parentBounds.size.x / transform.lossyScale.x) * 2f,
                (totalGeneratedHeight_Y / transform.lossyScale.y),
                (parentBounds.size.z / transform.lossyScale.z) * 2f
            );

            float offset_ToFeet = transform.parent.position.y - parentBounds.min.y;
            float newLocal_Y_Pos = (totalGeneratedHeight_Y / 2.0f) - offset_ToFeet;

            transform.localPosition = new Vector3(0, newLocal_Y_Pos / transform.parent.lossyScale.y, 0);
            myCollider.center = Vector3.zero;

            Debug.Log("Success: Trigger box auto-sized to 2x and positioned to sit on the object's base!");
        }
        else
        {
            Debug.LogWarning("Parent has no Renderer. Cannot auto-calculate size.");
        }
    }
#endif
}
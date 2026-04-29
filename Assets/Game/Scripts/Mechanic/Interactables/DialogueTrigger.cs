using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ChoiceVideoPair
{
    public string videoTag;
    public UnityEngine.Video.VideoClip videoClip;
}

[RequireComponent(typeof(BoxCollider))]
public class DialogueTrigger : BaseInteractable
{
    public TextAsset inkAsset;
    public UnityEngine.Video.VideoClip TriggerVideo;
    public GameObject interactPrompt;

    [Header("Choice Videos")]
    public List<ChoiceVideoPair> choiceVideos;

    public AudioClip voiceClip1;
    public AudioClip voiceClip2;
    public AudioClip voiceClip3;
    public AudioClip voiceClip4;

    public AudioSource voiceAudioSource;
    public bool playVoiceLines = true;

    public string symbolName = "";

    
    public override KeyCode InteractKey => KeyCode.G;

    private int currentDialogueLine = 0;

    protected override void Start()
    {
        base.Start();  
        GetComponent<Collider>().isTrigger = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

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

    private bool CanInteract()
    {
        if (DialogueManager.Instance == null || UIManager.Instance == null) return false;
        if (UIManager.Instance.dialoguePanel != null && UIManager.Instance.dialoguePanel.activeInHierarchy) return false;
        if (inkAsset == null) return false;

        if (!string.IsNullOrEmpty(symbolName))
        {
            if (symbolName.ToLower() == "boat")
            {
                if (MemorySymbolManager.Instance == null || !MemorySymbolManager.Instance.AreAllSymbolsInteracted())
                {
                    Debug.Log("[DialogueTrigger] Locked: Player needs to interact with all symbols first.");
                    return false;
                }
            }
            else
            {
                if (MemorySymbolManager.Instance != null && MemorySymbolManager.Instance.HasInteractedWithSymbol(symbolName))
                {
                    Debug.Log($"[DialogueTrigger] Locked: Player has already interacted with '{symbolName}'.");
                    return false;
                }
            }
        }
        return true;
    }

    public override void TogglePrompt(bool show)
    {
         
        if (interactPrompt != null)
        {
            Debug.Log("CanInteract() result is: " + CanInteract());
            interactPrompt.SetActive(show && CanInteract());
        }
    }

    public override void Interact()
    {
        if (!CanInteract()) return;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (TriggerVideo != null && GlobalCinematicManager.Instance != null)
        {
            DialogueManager.Instance.SetPlayerControl(false);
            GlobalCinematicManager.Instance.PlayCinematic(TriggerVideo, () =>
            {
                DialogueManager.Instance.StartDialogue(inkAsset, this);
            });
        }
        else
        {
            DialogueManager.Instance.StartDialogue(inkAsset, this);
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

    public UnityEngine.Video.VideoClip GetVideoByTag(string tag)
    {
        if (choiceVideos == null) return null;
        foreach (var cv in choiceVideos)
        {
            if (cv.videoTag.ToLower() == tag.ToLower())
                return cv.videoClip;
        }
        return null;
    }
}
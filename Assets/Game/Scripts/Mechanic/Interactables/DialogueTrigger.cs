using UnityEngine;
using System.Collections.Generic;

 
[System.Serializable]
public class ChoiceVideoPair
{
    public string videoTag;
    public UnityEngine.Video.VideoClip videoClip;
}

[RequireComponent(typeof(BoxCollider))]
public class DialogueTrigger : MonoBehaviour
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
    public AudioClip voiceClip5;
    public AudioClip voiceClip6;
    public AudioSource voiceAudioSource;
    public bool playVoiceLines = true;

    public string symbolName = "";

    private bool playerInRange = false;
    private int currentDialogueLine = 0;

    private void Start()
    {
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
                    if (!string.IsNullOrEmpty(symbolName) && symbolName.ToLower() == "boat")
                    {
                        if (MemorySymbolManager.Instance == null || !MemorySymbolManager.Instance.AreAllSymbolsInteracted())
                            return;
                    }

                    if (!string.IsNullOrEmpty(symbolName) && symbolName.ToLower() != "boat")
                    {
                        if (MemorySymbolManager.Instance != null &&
                            MemorySymbolManager.Instance.HasInteractedWithSymbol(symbolName))
                            return;
                    }

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
                if (!string.IsNullOrEmpty(symbolName) && MemorySymbolManager.Instance != null)
                {
                    if (MemorySymbolManager.Instance.HasInteractedWithSymbol(symbolName))
                        return;
                }

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
                interactPrompt.SetActive(false);
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
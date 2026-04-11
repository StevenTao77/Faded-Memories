using UnityEngine;

/// <summary>
/// Represents a single dialogue line with its associated voice file slots
/// </summary>
[System.Serializable]
public class DialogueVoiceLine
{
    [Tooltip("The INK tag or identifier for this dialogue line")]
    public string dialogueTag;

    [Tooltip("Audio clips for this dialogue line (supports up to 4 clips)")]
    public AudioClip[] voiceClips = new AudioClip[4];

    [Tooltip("Which voice clip index to use (0-3)")]
    [Range(0, 3)]
    public int activeVoiceIndex = 0;

    public DialogueVoiceLine()
    {
   voiceClips = new AudioClip[4];
        activeVoiceIndex = 0;
    }

    public AudioClip GetActiveVoiceClip()
    {
        if (activeVoiceIndex >= 0 && activeVoiceIndex < voiceClips.Length)
  {
            return voiceClips[activeVoiceIndex];
        }
 return null;
  }
}

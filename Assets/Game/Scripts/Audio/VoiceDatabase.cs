using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Database that maps dialogue lines to their voice files
/// Create one of these per dialogue scene or use a global one
/// </summary>
[CreateAssetMenu(fileName = "VoiceDatabase", menuName = "Dialogue/Voice Database")]
public class VoiceDatabase : ScriptableObject
{
    [SerializeField]
    private List<DialogueVoiceLine> voiceLines = new List<DialogueVoiceLine>();

    /// <summary>
    /// Get the voice line data for a specific dialogue tag
    /// </summary>
public DialogueVoiceLine GetVoiceLine(string dialogueTag)
    {
        foreach (var voiceLine in voiceLines)
  {
          if (voiceLine.dialogueTag == dialogueTag)
{
     return voiceLine;
            }
        }
        
        return null;
    }

 /// <summary>
    /// Get the active voice clip for a dialogue tag
    /// </summary>
    public AudioClip GetVoiceClip(string dialogueTag)
    {
      var voiceLine = GetVoiceLine(dialogueTag);
        return voiceLine != null ? voiceLine.GetActiveVoiceClip() : null;
    }

    /// <summary>
    /// Add or update a voice line
    /// </summary>
    public void AddOrUpdateVoiceLine(string dialogueTag, DialogueVoiceLine voiceLine)
    {
        int existingIndex = voiceLines.FindIndex(v => v.dialogueTag == dialogueTag);
        
        if (existingIndex >= 0)
        {
        voiceLines[existingIndex] = voiceLine;
     }
        else
        {
            voiceLines.Add(voiceLine);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public List<DialogueVoiceLine> GetAllVoiceLines()
    {
return voiceLines;
    }
}

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoiceDatabase))]
public class VoiceDatabaseEditor : Editor
{
    private Vector2 scrollPosition = Vector2.zero;

    public override void OnInspectorGUI()
    {
        VoiceDatabase database = (VoiceDatabase)target;

        EditorGUILayout.LabelField("Voice Database", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
      "Assign dialogue tags and their corresponding voice files here.\n" +
            "Tags should match the #TagName format in your INK files.\n" +
   "You can have up to 4 voice clips per dialogue line.",
        MessageType.Info
 );

    EditorGUILayout.Space();

        var voiceLines = database.GetAllVoiceLines();

        // Display each voice line
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < voiceLines.Count; i++)
        {
            EditorGUILayout.BeginHorizontal("box");

        EditorGUILayout.BeginVertical();

   // Dialogue Tag field
    voiceLines[i].dialogueTag = EditorGUILayout.TextField(
                "Dialogue Tag",
          voiceLines[i].dialogueTag
            );

            EditorGUILayout.Space(5);

     // Voice Clips
   EditorGUILayout.LabelField("Voice Clips", EditorStyles.boldLabel);
         for (int j = 0; j < voiceLines[i].voiceClips.Length; j++)
{
       voiceLines[i].voiceClips[j] = (AudioClip)EditorGUILayout.ObjectField(
         $"Clip {j + 1}",
             voiceLines[i].voiceClips[j],
       typeof(AudioClip),
     false
                );
      }

            EditorGUILayout.Space(5);

 // Active Voice Index selector
   EditorGUILayout.LabelField("Select Active Voice", EditorStyles.boldLabel);
            for (int j = 0; j < voiceLines[i].voiceClips.Length; j++)
 {
   EditorGUILayout.BeginHorizontal();
            GUI.enabled = voiceLines[i].voiceClips[j] != null;
          if (GUILayout.Button(
 voiceLines[i].activeVoiceIndex == j ? $"? Clip {j + 1}" : $"Clip {j + 1}",
     GUILayout.Height(25)
    ))
                {
        voiceLines[i].activeVoiceIndex = j;
             EditorUtility.SetDirty(database);
      }
              GUI.enabled = true;
       EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

         // Delete button
            EditorGUILayout.BeginVertical(GUILayout.Width(50));
  if (GUILayout.Button("Remove", GUILayout.Height(50)))
    {
    voiceLines.RemoveAt(i);
          EditorUtility.SetDirty(database);
      GUILayout.EndVertical();
       EditorGUILayout.EndHorizontal();
                continue;
            }
     EditorGUILayout.EndVertical();

   EditorGUILayout.EndHorizontal();
  EditorGUILayout.Space(10);
      }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        // Add new entry button
      if (GUILayout.Button("Add New Dialogue Voice Line", GUILayout.Height(30)))
        {
     voiceLines.Add(new DialogueVoiceLine());
         EditorUtility.SetDirty(database);
  }

        EditorGUILayout.Space();

        // Save changes
    if (GUI.changed)
        {
            EditorUtility.SetDirty(database);
        }
    }
}
#endif

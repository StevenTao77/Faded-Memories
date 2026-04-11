# Voice Files for Dialogue Lines - Setup Guide

## Overview
This system allows you to assign multiple voice files (up to 4) per dialogue line in your INK scripts. Each character can have different voice variations that can be switched in the Inspector.

## Components Created

### 1. **DialogueVoiceLine.cs**
- Serializable class that holds up to 4 audio clips
- Tracks which clip is currently active
- Used for organizing voice data per dialogue tag

### 2. **VoiceDatabase.cs**
- ScriptableObject that manages all voice assignments
- Maps dialogue tags (from INK #TagName) to voice clips
- Can be created per scene or used globally

### 3. **VoiceDatabaseEditor.cs**
- Custom Inspector for easy voice file assignment
- Visual UI for selecting which voice variant is active
- One-click removal of dialogue entries

## Setup Steps

### Step 1: Create a Voice Database
1. Right-click in your project folder (e.g., Assets/Game/Audio/)
2. Select **Create > Dialogue > Voice Database**
3. Name it something like `VoiceDatabase_Level01` or `GlobalVoiceDatabase`

### Step 2: Configure DialogueManager
1. Open your DialogueManager in the Inspector
2. Drag the VoiceDatabase into the "Voice Database" field
3. Assign an AudioSource (or let it auto-create one)
4. Toggle "Play Voice Lines" to enable/disable voice playback

### Step 3: Add Voice Lines to the Database
1. Open your VoiceDatabase in the Inspector
2. Click "Add New Dialogue Voice Line"
3. Enter the **Dialogue Tag** (must match INK tags)

Example INK file:
```
Home. #Character_Old_Man
Altough I tend to feel lonely from time to time. #Character_Old_Man
```

The tag here is `Character_Old_Man`

### Step 4: Assign Voice Clips
1. For each dialogue tag, you can assign up to 4 voice clips
2. Click the button next to the clip you want to use (indicated with ?)
3. Only the selected clip will play during dialogue

## How It Works

### INK Tags
In your INK files, use tags to identify dialogue lines:

```ink
=== start_node ===
Character name. #Character_OldMan
This is the dialogue line. #Character_OldMan
-> END
```

### Switching Between Voice Variants
If you have multiple voice actors or variations:

1. Add all 4 clips to the "Voice Clips" array
2. Use the "Select Active Voice" buttons to choose which one to use
3. Switch between them anytime without reassigning clips

## Example Usage

### INK File (Assets/Game/Scripts/Dialog/story.ink)
```
=== start_node ===
Home. #NPC_Farmer
Altough I tend to feel lonely from time to time. #NPC_Farmer
I wouldn't want to live anywhere else. #NPC_Farmer
-> END
```

### Voice Database Setup
```
Dialogue Tag: NPC_Farmer
Clip 1: farmer_voice_male_v1.wav
Clip 2: farmer_voice_male_v2.wav
Clip 3: (empty)
Clip 4: (empty)
Active: Clip 1 (selected)
```

## Advanced Features

### Multiple Characters in One Scene
Create one VoiceDatabase and add entries for each character:
- `NPC_Farmer`
- `NPC_Merchant`
- `Player`
- etc.

### Scene-Specific Voices
For multi-scene games, create separate databases:
- `VoiceDatabase_Level01`
- `VoiceDatabase_Level02`
- Assign the correct one to DialogueManager per scene

### Switching Voice Variants
To switch between voice variants at runtime:

```csharp
// In your game manager or script
voiceDatabase.GetVoiceLine("NPC_Farmer").activeVoiceIndex = 1; // Switch to Clip 2
```

## Troubleshooting

### Voice Not Playing
1. Check that "Play Voice Lines" is enabled
2. Verify the tag in your INK file matches the tag in the database
3. Confirm the AudioClip is assigned
4. Check that an AudioSource exists on the DialogueManager

### Wrong Voice Clip Playing
- Make sure the correct clip is selected (? button)
- Verify the tag spelling matches exactly

### Multiple Clips Playing
- Ensure only one variant is selected per tag
- The active voice index should only be one value (0-3)

## File Locations
- Voice Database: Assets/Game/Audio/ (or your audio folder)
- Code Files:
  - Assets/Game/Scripts/Audio/DialogueVoiceLine.cs
  - Assets/Game/Scripts/Audio/VoiceDatabase.cs
  - Assets/Game/Scripts/Audio/VoiceDatabaseEditor.cs

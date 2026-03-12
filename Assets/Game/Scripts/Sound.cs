using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [HideInInspector]
    public AudioSource source;

    public AudioMixerGroup mixerGroup;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0f, 1f)]
    public float volumeVariance = 0f;

    [Range(.1f, 3f)]
    public float pitch = 1f;
    [Range(0f, 1f)]
    public float pitchVariance = 0f;

    public bool loop = false;
    public bool spatialize = true;

    [Header("Spatialization Parameters")]
    public float minDistance = 1.5f;
    public float maxDistance = 3.0f;
    [Range(0f, 360f)]
    public float spread = 0f;

}

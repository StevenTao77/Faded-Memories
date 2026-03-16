using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    public Sound[] sounds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    private Sound GetSound(string soundName)
    {
        Sound s = Array.Find(sounds, item => item.name == soundName);

        if (s == null)
        {
            Debug.LogWarning("Sound named: " + soundName + " not found!");
        }
        return s;
    }
    private void SetAudioStats(Sound s, GameObject emitter)
    {
        if (emitter.GetComponent<AudioSource>() != null)
        {
            s.source = emitter.GetComponent<AudioSource>();
        }
        else
        {
            emitter.AddComponent<AudioSource>();
            s.source = emitter.GetComponent<AudioSource>();
        }

        s.source.clip = s.clip;
        s.source.loop = s.loop;

        if (s.spatialize)
        {
            s.source.spatialize = true;
            s.source.rolloffMode = AudioRolloffMode.Linear;
            s.source.spatialBlend = 1f;
            s.source.minDistance = s.minDistance;
            s.source.maxDistance = s.maxDistance;
            s.source.spread = s.spread;
        }

        s.source.outputAudioMixerGroup = s.mixerGroup;

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
    }
    // Play cannot overlap with itself and PlayOneShot can
    public void Play(string sound, GameObject emitter)
    {
        Sound s = GetSound(sound);
        if (s == null) return;

        SetAudioStats(s, emitter);
        s.source.Play();
    }
    public void PlayOneShot(string sound, GameObject emitter)
    {
        Sound s = GetSound(sound);
        if (s == null) return;

        SetAudioStats(s, emitter);
        s.source.PlayOneShot(s.clip);
    }
    public void Stop(string sound, GameObject emitter)
    {
        Sound s = GetSound(sound);
        if (s == null) return;

        SetAudioStats(s, emitter);
        s.source.Stop();
    }

    // Fade in and Fade out
    private IEnumerator Fade(AudioSource source, float duration, float targetVolume)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        source.volume = targetVolume;
    }
    private IEnumerator FadeOutAndStop(AudioSource source, float duration)
    {
        yield return Fade(source, duration, 0f);
        source.Stop();
    }
    public void Fadein(string soundName, float fadeTime, GameObject emitter)
    {
        Sound s = GetSound(soundName);
        if (s == null) return;

        SetAudioStats(s, emitter);
        s.source.volume = 0;
        s.source.Play();
        Debug.Log("Started to fade in " + s.name);

        StartCoroutine(Fade(s.source, fadeTime, s.volume));
    }
    public void FadeOutAndStop(string soundName, float fadeTime)
    {
        Sound s = GetSound(soundName);
        if (s == null) return;

        Debug.Log("Started to fade out " + s.name);

        StartCoroutine(FadeOutAndStop(s.source, fadeTime));
    }
}


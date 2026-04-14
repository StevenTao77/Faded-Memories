using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Toggle masterToggle;
    private bool masterVolumeMuted = false;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle sfxToggle;
    private bool sfxVolumeMuted = false;

    [SerializeField] private Slider ambienceSlider;
    [SerializeField] private Toggle ambienceToggle;
    private bool ambienceVolumeMuted = false;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle musicToggle;
    private bool musicVolumeMuted = false;

    void Start()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume();
            SetSFXVolume();
            SetAmbienceVolume();
            SetMusicVolume();
        }
    }

    public void SetMasterVolume()
    {
        if (masterVolumeMuted == false)
        {
            Debug.Log("Master Volume is: " + masterSlider.value);
            float volume = masterSlider.value;
            mixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20f);
            PlayerPrefs.SetFloat("masterVolume", volume);
        }
        else
        {
            Debug.Log("Master Volume muted");
        }
    }
    public void MuteMasterVolume()
    {
        if (masterVolumeMuted == false)
        {
            masterVolumeMuted = true;
            Debug.Log("Muted master volume");
            mixer.SetFloat("masterVolume", Mathf.Log10(0.0001f) * 20f);
            PlayerPrefs.SetInt("masterVolumeMuted", 1);
        }
        else
        {
            masterVolumeMuted = false;
            Debug.Log("Unmuted master volume ");
            mixer.SetFloat("masterVolume", Mathf.Log10(masterSlider.value) * 20f);
            PlayerPrefs.SetInt("masterVolumeMuted", (masterVolumeMuted ? 1 : 0));
        }
    }

    public void SetSFXVolume()
    {
        if (sfxVolumeMuted == false)
        {
            Debug.Log("SFX Volume is: " + sfxSlider.value);
            float volume = sfxSlider.value;
            mixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20f);
            PlayerPrefs.SetFloat("sfxVolume", volume);
        }
        else
        {
            Debug.Log("SFX Volume muted");
        }
    }
    public void MuteSFXVolume()
    {
        if (sfxVolumeMuted == false)
        {
            sfxVolumeMuted = true;
            Debug.Log("Muted sfx volume");
            mixer.SetFloat("sfxVolume", Mathf.Log10(0.0001f) * 20f);
            PlayerPrefs.SetInt("sfxVolumeMuted", 1);
        }
        else
        {
            sfxVolumeMuted = false;
            Debug.Log("Unmuted sfx volume");
            mixer.SetFloat("sfxVolume", Mathf.Log10(sfxSlider.value) * 20f);
            PlayerPrefs.SetInt("sfxVolumeMuted", (sfxVolumeMuted ? 1 : 0));
        }
    }

    public void SetAmbienceVolume()
    {
        if (ambienceVolumeMuted == false)
        {
            Debug.Log("Ambience Volume is: " + ambienceSlider.value);
            float volume = ambienceSlider.value;
            mixer.SetFloat("ambienceVolume", Mathf.Log10(volume) * 20f);
            PlayerPrefs.SetFloat("ambienceVolume", volume);
        }
        else
        {
            Debug.Log("Ambience Volume muted");
        }
    }
    public void MuteAmbienceVolume()
    {
        if (ambienceVolumeMuted == false)
        {
            ambienceVolumeMuted = true;
            Debug.Log("Muted ambience volume");
            mixer.SetFloat("ambienceVolume", Mathf.Log10(0.0001f) * 20f);
            PlayerPrefs.SetInt("ambienceVolumeMuted", 1);
        }
        else
        {
            ambienceVolumeMuted = false;
            Debug.Log("Unmuted ambience volume");
            mixer.SetFloat("ambienceVolume", Mathf.Log10(ambienceSlider.value) * 20f);
            PlayerPrefs.SetInt("ambienceVolumeMuted", (ambienceVolumeMuted ? 1 : 0));
        }
    }

    public void SetMusicVolume()
    {
        if (musicVolumeMuted == false)
        {
            Debug.Log("Music Volume is: " + musicSlider.value);
            float volume = musicSlider.value;
            mixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20f);
            PlayerPrefs.SetFloat("musicVolume", volume);
        }
        else
        {
            Debug.Log("Music Volume muted");
        }
    }
    public void MuteMusicVolume()
    {
        if (musicVolumeMuted == false)
        {
            musicVolumeMuted = true;
            Debug.Log("Muted music volume");
            mixer.SetFloat("musicVolume", Mathf.Log10(0.0001f) * 20f);
            PlayerPrefs.SetInt("musicVolumeMuted", 1);
        }
        else
        {
            musicVolumeMuted = false;
            Debug.Log("Unmuted music volume");
            mixer.SetFloat("musicVolume", Mathf.Log10(musicSlider.value) * 20f);
            PlayerPrefs.SetInt("musicVolumeMuted", (musicVolumeMuted ? 1 : 0));
        }
    }

    private void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        masterToggle.isOn = PlayerPrefs.GetInt("masterVolumeMuted") == 0;

        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        sfxToggle.isOn = PlayerPrefs.GetInt("sfxVolumeMuted") == 0;

        ambienceSlider.value = PlayerPrefs.GetFloat("ambienceVolume");
        ambienceToggle.isOn = PlayerPrefs.GetInt("ambienceVolumeMuted") == 0;

        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        musicToggle.isOn = PlayerPrefs.GetInt("musicVolumeMuted") == 0;

        SetMasterVolume();
        SetSFXVolume();
        SetAmbienceVolume();
        SetMusicVolume();
    }
}

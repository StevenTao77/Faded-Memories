using UnityEngine;
using UnityEngine.Audio;

public class MainMenuMusic : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    [SerializeField]private GameObject music;
    [SerializeField]private GameObject ambience;
    void Start()
    {
        LoadSavedVolumes();
        SoundFXManager.instance.Fadein("PianoBackground", 1f, music);
        SoundFXManager.instance.Fadein("NightSoundsWater", 1f, ambience);
    }

    public void PlayClickSound() 
    {
        SoundFXManager.instance.Play("UiClick", gameObject);
    }
    private void LoadSavedVolumes() 
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            mixer.SetFloat("masterVolume", Mathf.Log10(PlayerPrefs.GetFloat("masterVolume")) * 20f);
            mixer.SetFloat("sfxVolume", Mathf.Log10(PlayerPrefs.GetFloat("sfxVolume")) * 20f);
            mixer.SetFloat("ambienceVolume", Mathf.Log10(PlayerPrefs.GetFloat("ambienceVolume")) * 20f);
            mixer.SetFloat("musicVolume", Mathf.Log10(PlayerPrefs.GetFloat("musicVolume")) * 20f);
            Debug.Log("Saves sound volumes loaded");
        }
    }
}

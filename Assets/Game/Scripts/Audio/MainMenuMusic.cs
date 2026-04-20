using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    [SerializeField]private GameObject music;
    [SerializeField]private GameObject ambience;
    void Start()
    {
        SoundFXManager.instance.Fadein("PianoBackground", 1f, music);
        SoundFXManager.instance.Fadein("NightSoundsWater", 1f, ambience);
    }

    public void PlayClickSound() 
    {
        SoundFXManager.instance.Play("UiClick", gameObject);
    }
}

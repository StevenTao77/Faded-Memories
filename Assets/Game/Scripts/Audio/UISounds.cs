using UnityEngine;

public class UISounds : MonoBehaviour
{
    [SerializeField] GameObject UISoundEmmitter;
    public void PlayClickSound()
    {
        SoundFXManager.instance.PlayOneShot("UiClick", UISoundEmmitter);
    }
    public void PlayHoverSound()
    {
        SoundFXManager.instance.PlayOneShot("UiHover", UISoundEmmitter);
    }
}

using UnityEngine;

public class TorchAudioHandler : MonoBehaviour
{
    public GameObject gramps;
    private void OnEnable()
    {
        SoundFXManager.instance.PlayOneShot("Equip", gramps);
        //SoundFXManager.instance.PlayOneShot("Flames", gameObject);
    }

    private void OnDisable()
    {
        SoundFXManager.instance.PlayOneShot("Unequip", gramps);
        //SoundFXManager.instance.FadeOutAndStop("Flames", 2);
    }
}

using System;
using UnityEngine;
using System.Collections;

public class ItemEquipAudioHandler : MonoBehaviour
{
    public GameObject gramps;
    public GameObject hand;
    public GameObject hand2;
    public GameObject torchFlameAndLight;

    private void OnEnable()
    {
        SoundFXManager.instance.PlayOneShot("Equip", gramps);

        if (gameObject.name == "Torch")
        {
            StartCoroutine(PlayTorchSounds());
        }
    }

    private void OnDisable()
    {
        SoundFXManager.instance.PlayOneShot("Unequip", gramps);

        if (gameObject.name == "Torch")
        {
            SoundFXManager.instance.FadeOutAndStop("Flames", 1);
            torchFlameAndLight.SetActive(false);
        }
    }

    private IEnumerator PlayTorchSounds()
    {
        SoundFXManager.instance.PlayOneShot("FlamesIgnite", hand);

        yield return new WaitForSeconds(1f);

        SoundFXManager.instance.PlayOneShot("Flames", hand2);
        torchFlameAndLight.SetActive(true);
        Debug.Log("Playing flames sound after delay");
    }
}

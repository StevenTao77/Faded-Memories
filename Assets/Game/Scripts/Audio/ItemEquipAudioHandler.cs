using System;
using UnityEngine;

public class ItemEquipAudioHandler : MonoBehaviour
{
    public GameObject gramps;
    public GameObject hand;
    String gameObjectName;
    
    private void OnEnable()
    {
        String gameObjectName = gameObject.name;
        SoundFXManager.instance.PlayOneShot("Equip", gramps);

        if (gameObjectName == "Torch")
        {
            SoundFXManager.instance.Fadein("Flames", 3, hand);
            Debug.Log("Playing flames sound for " + gameObjectName);
        }
    }

    private void OnDisable()
    {
        String gameObjectName = gameObject.name;
        SoundFXManager.instance.PlayOneShot("Unequip", gramps);

        if (gameObjectName == "Torch")
        {
            SoundFXManager.instance.FadeOutAndStop("Flames", 1);
        }
    }
}

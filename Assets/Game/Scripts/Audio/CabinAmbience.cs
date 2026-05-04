using UnityEngine;

public class CabinAmbience : MonoBehaviour
{
    [SerializeField]
    private GameObject firePit;
    void Start()
    {
        SoundFXManager.instance.Play("Fire", firePit);
    }
}

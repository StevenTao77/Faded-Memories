using UnityEngine;

public class FootStepRandomizer : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip[] FootStepClips;

    private void OnEnable()
    {
        AudioClip randomClip = FootStepClips[Random.Range(0, FootStepClips.Length)];
        AudioSource.PlayOneShot(randomClip);
    }

}

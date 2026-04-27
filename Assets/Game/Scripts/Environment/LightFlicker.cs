using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light lightToFlicker;
    [SerializeField, Range(0f, 200f)] private float minIntensity = 0.5f;
    [SerializeField, Range(0f, 200f)] private float maxIntensity = 5f;
    [SerializeField, Min(0f)] private float timebetweenIntensity = 0.1f;

    private float currentTimer;

    private void Awake()
    {
        if (lightToFlicker == null)
        {
            lightToFlicker = GetComponent<Light>();
        }
        ValidateIntensityBounds();
    }

    private void Update()
    {
        currentTimer += Time.deltaTime;
        if (!(currentTimer >= timebetweenIntensity)) return;
        lightToFlicker.intensity = Random.Range(minIntensity, maxIntensity);
        currentTimer = 0f;
    }

    private void ValidateIntensityBounds()
    {
        if (!(minIntensity > maxIntensity)) return;
        Debug.LogWarning("Min intensity is greater than max intensity, swapping values.");
        (minIntensity, maxIntensity) = (maxIntensity, minIntensity);
    }
}

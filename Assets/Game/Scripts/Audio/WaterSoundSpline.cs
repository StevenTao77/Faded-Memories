using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class WaterSoundSpline : MonoBehaviour
{
    [SerializeField]
    private SplineContainer Spline;
    [SerializeField]
    private Transform Player;
    [SerializeField]
    private string ambienceName = "";
    private void Start()
    {
        SoundFXManager.instance.Fadein(ambienceName, 1f, gameObject);
    }

    void Update()
    {
        // Convert Player position to spline local space
        Vector3 localPlayerPos = Spline.transform.InverseTransformPoint(Player.position);

        // Find nearest point on spline
        SplineUtility.GetNearestPoint(Spline.Spline, localPlayerPos, out float3 nearestPointLocal, out float normalizedT);

        // Convert back to world space
        Vector3 nearestWorldPos = Spline.transform.TransformPoint(nearestPointLocal);

        // Set object position and rotation
        transform.position = nearestWorldPos;
        Vector3 tangent = Spline.transform.TransformDirection(Spline.Spline.EvaluateTangent(normalizedT));
        if (tangent != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(tangent);
    }
}

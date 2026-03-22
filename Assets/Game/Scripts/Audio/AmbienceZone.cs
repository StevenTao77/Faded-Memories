using UnityEngine;

public class AmbienceZone : MonoBehaviour
{
    public Collider area;
    public GameObject player;
    public string ambienceName = "";

    void Start()
    {
        if (ambienceName == "")
        {
            Debug.Log("Missing name from" + gameObject);
        }
        SoundFXManager.instance.Fadein(ambienceName, 1f, gameObject);
    }
    void Update()
    {
        if (player != null) 
        {
            Vector3 closestPoint = area.ClosestPoint(player.transform.position);
            transform.position = closestPoint;
        }
        else 
        {
            Debug.LogWarning("Missing player on: " + gameObject);
        }
    }
}

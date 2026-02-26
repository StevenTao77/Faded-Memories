using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("Collision Settings")]
    [Tooltip("Enter the index of the scene you want to load when the player collides with this object.")]
    public int targetSceneIndexOnCollision;

    // Method 1: Called by UI Buttons or other scripts
    public void LoadScene(int sceneIndex)
    {
        // Optional: Safety check to prevent crashing if you type a wrong number
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError($"Scene Index {sceneIndex} is out of range! Check your Build Settings.");
        }
    }

    // Method 2: Triggered automatically by physics collision
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object colliding with this trigger has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Reuse your safe loading method
            LoadScene(targetSceneIndexOnCollision);
        }
    }
}
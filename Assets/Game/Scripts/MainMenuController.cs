using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Method 1: Load a specific index (Hardcoded)
     
    public void LoadScene(int sceneIndex)
    {
        // Optional: Safety check to prevent crashing if you type a wrong number
        if (sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError($"Scene Index {sceneIndex} is out of range! Check your Build Settings.");
        }
    }

     
    
}
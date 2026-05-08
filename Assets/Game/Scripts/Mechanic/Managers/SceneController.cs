using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("Collision Settings")] 
    public int targetSceneIndexOnCollision;
     

    //private void Start()
    //{
    //    Cursor.visible = true;
    //    Cursor.lockState = CursorLockMode.None;
    //}



    public void LoadScene(int sceneIndex)
    { 
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError($"Scene Index {sceneIndex} is out of range! Check your Build Settings.");
        }
    }
     
    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Player"))
        { 
            LoadScene(targetSceneIndexOnCollision);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
 
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneLoader : MonoBehaviour
{
    void Start()
    { 

        LoadSceneIfNotLoaded("Island_environment");
       
        LoadSceneIfNotLoaded("Island_mechanic");
         LoadSceneIfNotLoaded("Island_UI");
    }

    private void LoadSceneIfNotLoaded(string sceneName)
    {
        // Prevent loading the same scene twice if you restart the game
        bool isSceneLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name == sceneName)
            {
                isSceneLoaded = true;
                break;
            }
        }

        if (!isSceneLoaded)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }
}
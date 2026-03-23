using UnityEngine;
using UnityEngine.SceneManagement;

public class Cabin_insideSceneLoader : MonoBehaviour
{
    void Start()
    {
        // Load the scenes additively so they stack on top of Island_main
        // IMPORTANT: The spelling must exactly match your scene file names!

        LoadSceneIfNotLoaded("Cabin_inside_envir");
        LoadSceneIfNotLoaded("Cabin_inside_mechanic");
        LoadSceneIfNotLoaded("Cabin_inside_UI");
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
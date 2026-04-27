using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage;
    public float speed = 2f;

    public string[] islandScenes = {
        "Island_main",
        "Island_mechanic",
        "Island_UI",
        "Island_environment"
    };

    public void FadeAndLoad(string targetScene)
    {
        StartCoroutine(FadeRoutine(targetScene));
    }

    IEnumerator FadeRoutine(string targetScene)
    {
        float a = 0;
        while (a < 1)
        {
            a += Time.deltaTime * speed;
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }

        yield return SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);

        foreach (string sceneName in islandScenes)
        {
            if (SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                SceneManager.UnloadSceneAsync(sceneName);
            }
        }

        Scene newScene = SceneManager.GetSceneByName(targetScene);
        SceneManager.SetActiveScene(newScene);

        a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime * speed;
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }
    }

    void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }
}
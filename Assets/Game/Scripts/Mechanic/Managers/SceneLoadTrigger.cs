using UnityEngine;

public class SceneLoadTrigger : MonoBehaviour
{
    public void RequestSceneLoad(int sceneIndex)
    {
        SceneController controller = FindAnyObjectByType<SceneController>();

        if (controller != null)
        {
            controller.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError("SceneController not found! Make sure the mechanic scene is loaded and contains the GameManager.");
        }
    }
}
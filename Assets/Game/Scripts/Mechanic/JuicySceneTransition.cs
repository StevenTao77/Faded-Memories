using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JuicySceneTransition : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("BlackScreen object.")]
    public CanvasGroup transitionCanvasGroup;

    [Header("Juice Settings")]
    public float fadeDuration = 1.2f;
    [Tooltip("Curve.")]
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private void Start()
    {
        if (transitionCanvasGroup != null)
        {
            // Start completely black, then fade to clear
            transitionCanvasGroup.alpha = 1f;
            StartCoroutine(FadeInRoutine());
        }
    }

    public void LoadSceneWithFade(int sceneIndex)
    {
        StartCoroutine(FadeOutAndLoadRoutine(sceneIndex));
    }

    private IEnumerator FadeInRoutine()
    {
        transitionCanvasGroup.blocksRaycasts = true;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            // Evaluate the curve for a non-linear, juicy fade
            transitionCanvasGroup.alpha = 1f - fadeCurve.Evaluate(progress);
            yield return null;
        }

        transitionCanvasGroup.alpha = 0f;
        // Allow clicking on other UI elements again
        transitionCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator FadeOutAndLoadRoutine(int sceneIndex)
    {
        // Block screen clicks during transition
        transitionCanvasGroup.blocksRaycasts = true;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            transitionCanvasGroup.alpha = fadeCurve.Evaluate(progress);
            yield return null;
        }

        transitionCanvasGroup.alpha = 1f;

        // Load the new scene strictly after the screen is fully black
        SceneManager.LoadScene(sceneIndex);
    }
}
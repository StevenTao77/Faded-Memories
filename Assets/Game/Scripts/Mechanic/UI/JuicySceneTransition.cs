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

             
            transitionCanvasGroup.alpha = 1f - fadeCurve.Evaluate(progress);
            yield return null;
        }

        transitionCanvasGroup.alpha = 0f;
         
        transitionCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator FadeOutAndLoadRoutine(int sceneIndex)
    {
         
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

        

        //if (MemorySymbolManager.Instance != null)
        //{
        //    MemorySymbolManager.Instance.ResetProgress();
        //}

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleDialoguePanel(false);
            if (UIManager.Instance.dialogueText != null)
            {
                UIManager.Instance.dialogueText.text = "";
            }
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.SetPlayerControl(true);
            DialogueManager.Instance.ForceEndDialogue();
        }

        Time.timeScale = 1f;
        
        SceneManager.LoadScene(sceneIndex);
    }
}
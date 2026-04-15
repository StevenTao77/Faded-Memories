using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using System.Collections;

public class IntroVideoManager : MonoBehaviour
{
    [Header("Intro Video Settings")]
    public VideoPlayer introVideoPlayer;
    public GameObject videoScreenUI;

    [Header("Cinematic Transition Settings")]
    public CanvasGroup blackScreenCanvasGroup;
    public float fadeToBlackDuration = 0.8f;
    public float fadeFromBlackDuration = 1.0f;
    public float skipFadeOutDuration = 0.5f;

    [Header("After Video Event")]
    public UnityEvent onVideoFinished;
    public bool skipAllowed = true;

    private CanvasGroup videoCanvasGroup;
    private bool isVideoActive = false;
    private bool isTransitioning = false;

    private void Start()
    { 
        if (introVideoPlayer != null)
        {
            introVideoPlayer.loopPointReached += OnVideoEndReached;
        }
         
        if (videoScreenUI != null)
        {
            videoCanvasGroup = videoScreenUI.GetComponent<CanvasGroup>();
            if (videoCanvasGroup == null)
            {
                videoCanvasGroup = videoScreenUI.AddComponent<CanvasGroup>();
            } 

            videoCanvasGroup.alpha = 1f;
            videoScreenUI.SetActive(false);
        }
         
        if (blackScreenCanvasGroup != null)
        {
            blackScreenCanvasGroup.alpha = 0f;
            blackScreenCanvasGroup.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Listen for skip key only when video is active  
        if (isVideoActive && !isTransitioning && Input.GetKeyDown(KeyCode.Q))
        {
            SkipVideo();
        }
    }
     
    public void PlayIntroVideo()
    {
        if (introVideoPlayer != null && videoScreenUI != null && !isVideoActive && !isTransitioning)
        {
            StartCoroutine(CinematicPlaySequence());
             
        }
    }

    private IEnumerator CinematicPlaySequence()
    {
        isTransitioning = true;
        isVideoActive = true;

        //Fade the black screen IN to cover the menu
        if (blackScreenCanvasGroup != null)
        {
            blackScreenCanvasGroup.gameObject.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(blackScreenCanvasGroup, 0f, 1f, fadeToBlackDuration));
        }

        //Prepare the video behind the black screen to avoid stuttering
        videoScreenUI.SetActive(true);
        introVideoPlayer.Prepare();

        while (!introVideoPlayer.isPrepared)
        {
            yield return null;
        }

        introVideoPlayer.Play();
         
        if (blackScreenCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(blackScreenCanvasGroup, 1f, 0f, fadeFromBlackDuration));
            blackScreenCanvasGroup.gameObject.SetActive(false);
        }

        isTransitioning = false;
    }

    private void SkipVideo()
    {
        if (introVideoPlayer != null && !isTransitioning)
        {
            StartCoroutine(CinematicEndSequence());
        }
    }

    // Handles both skipping and natural ending with a smooth fade to black
    private IEnumerator CinematicEndSequence()
    {
        isTransitioning = true;
         
        if (blackScreenCanvasGroup != null)
        {
            blackScreenCanvasGroup.gameObject.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(blackScreenCanvasGroup, 0f, 1f, skipFadeOutDuration));
        }
         
        if (introVideoPlayer != null)
        {
            introVideoPlayer.Pause();
        }

        FinishVideoSequence();
    }

    private void FinishVideoSequence()
    {
        if (!isVideoActive) return;

        isVideoActive = false;
        isTransitioning = false;

       
        onVideoFinished?.Invoke();
    }

    private void OnVideoEndReached(VideoPlayer vp)
    {
        if (!isTransitioning)
        {
            StartCoroutine(CinematicEndSequence());
        }
    }

     
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        cg.alpha = startAlpha;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration));
            yield return null;
        }

        cg.alpha = endAlpha;
    }

    private void OnDestroy()
    {
        if (introVideoPlayer != null)
        {
            introVideoPlayer.loopPointReached -= OnVideoEndReached;
        }
    }
}
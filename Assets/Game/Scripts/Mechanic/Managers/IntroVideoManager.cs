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
    [Tooltip("Assign the CanvasGroup from your existing TransitionCanvas here.")]
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
        // Subscribe to the event when video reaches the end
        if (introVideoPlayer != null)
        {
            introVideoPlayer.loopPointReached += OnVideoEndReached;
        }

        // Initialize Video Screen UI
        if (videoScreenUI != null)
        {
            videoCanvasGroup = videoScreenUI.GetComponent<CanvasGroup>();
            if (videoCanvasGroup == null)
            {
                videoCanvasGroup = videoScreenUI.AddComponent<CanvasGroup>();
            }
            // Keep video UI fully opaque, we use the black screen to mask it
            videoCanvasGroup.alpha = 1f;
            videoScreenUI.SetActive(false);
        }

        // Ensure the transition black screen is invisible at start
        if (blackScreenCanvasGroup != null)
        {
            blackScreenCanvasGroup.alpha = 0f;
            blackScreenCanvasGroup.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Listen for skip key only when video is active and not currently fading
        if (isVideoActive && !isTransitioning && Input.GetKeyDown(KeyCode.Q))
        {
            SkipVideo();
        }
    }

    // Called by the "Play" button in Main Menu
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

        // 1. Fade the black screen IN to cover the menu
        if (blackScreenCanvasGroup != null)
        {
            blackScreenCanvasGroup.gameObject.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(blackScreenCanvasGroup, 0f, 1f, fadeToBlackDuration));
        }

        // 2. Prepare the video behind the black screen to avoid stuttering
        videoScreenUI.SetActive(true);
        introVideoPlayer.Prepare();

        while (!introVideoPlayer.isPrepared)
        {
            yield return null;
        }

        introVideoPlayer.Play();

        // 3. Fade the black screen OUT to reveal the video
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

        // 1. Fade to black BEFORE the video stops to hide the cut
        if (blackScreenCanvasGroup != null)
        {
            blackScreenCanvasGroup.gameObject.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(blackScreenCanvasGroup, 0f, 1f, skipFadeOutDuration));
        }

        // 2. Pause the video to freeze the frame instead of Stop() to prevent flicker
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

        // NOTE: We do NOT set videoScreenUI to inactive here. 
        // This ensures the video (or its last frame) keeps covering the menu 
        // until the next scene is fully loaded.

        onVideoFinished?.Invoke();
    }

    private void OnVideoEndReached(VideoPlayer vp)
    {
        if (!isTransitioning)
        {
            StartCoroutine(CinematicEndSequence());
        }
    }

    // Helper method to interpolate CanvasGroup alpha over time
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
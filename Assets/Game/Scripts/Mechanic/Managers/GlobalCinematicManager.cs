using UnityEngine;
using UnityEngine.Video;
using System;
using System.Collections;

public class GlobalCinematicManager : MonoBehaviour
{
    public static GlobalCinematicManager Instance { get; private set; }

    [Header("Direct UI References")]
    [Tooltip("Drag the BlackOverlay (CanvasGroup) here")]
    public CanvasGroup blackScreenCanvasGroup;

    [Tooltip("Drag the VideoScreen (RawImage) here")]
    public GameObject videoScreenUI;

    [Tooltip("Drag the Video Player component here")]
    public VideoPlayer globalVideoPlayer;

    [Header("Transition Settings")]
    public float fadeDuration = 0.8f;

    private Action onVideoCompleteCallback;
    private bool isPlaying = false;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (globalVideoPlayer != null)
        {
            globalVideoPlayer.loopPointReached += OnVideoEndReached;
        }

        // Initialize UI states to hidden
        if (videoScreenUI != null) videoScreenUI.SetActive(false);
        if (blackScreenCanvasGroup != null)
        {
            blackScreenCanvasGroup.alpha = 0f;
            blackScreenCanvasGroup.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlaying && !isTransitioning && Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(EndCinematicSequence());
        }
    }

    public void PlayCinematic(VideoClip clip, Action onComplete)
    {
        if (isPlaying || isTransitioning) return;
        StartCoroutine(StartCinematicSequence(clip, onComplete));
    }

    private IEnumerator StartCinematicSequence(VideoClip clip, Action onComplete)
    {
        isPlaying = true;
        isTransitioning = true;
        onVideoCompleteCallback = onComplete;

        // 1. Fade OUT to black
        if (blackScreenCanvasGroup != null)
        {
            blackScreenCanvasGroup.gameObject.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(blackScreenCanvasGroup, 0f, 1f, fadeDuration));
        }

        // 2. Prepare the video
        globalVideoPlayer.clip = clip;
        videoScreenUI.SetActive(true);
        globalVideoPlayer.Prepare();

        while (!globalVideoPlayer.isPrepared)
        {
            yield return null;
        }

        // 3. Play and reveal
        globalVideoPlayer.Play();
        if (blackScreenCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(blackScreenCanvasGroup, 1f, 0f, fadeDuration));
            blackScreenCanvasGroup.gameObject.SetActive(false);
        }

        isTransitioning = false;
    }

    private void OnVideoEndReached(VideoPlayer vp)
    {
        if (!isTransitioning)
        {
            StartCoroutine(EndCinematicSequence());
        }
    }

    private IEnumerator EndCinematicSequence()
    {
        isTransitioning = true;

        // 1. Fade OUT to black
        if (blackScreenCanvasGroup != null)
        {
            blackScreenCanvasGroup.gameObject.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(blackScreenCanvasGroup, 0f, 1f, fadeDuration));
        }

        // 2. Stop video
        globalVideoPlayer.Stop();
        videoScreenUI.SetActive(false);

        // 3. Trigger Dialogue
        onVideoCompleteCallback?.Invoke();
        onVideoCompleteCallback = null;
        isPlaying = false;

        // 4. Fade IN from black
        if (blackScreenCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(blackScreenCanvasGroup, 1f, 0f, fadeDuration));
            blackScreenCanvasGroup.gameObject.SetActive(false);
        }

        isTransitioning = false;
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
        if (globalVideoPlayer != null)
        {
            globalVideoPlayer.loopPointReached -= OnVideoEndReached;
        }
    }
}
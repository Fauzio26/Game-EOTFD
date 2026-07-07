using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class StoryManager : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer storyVideoPlayer;
    public GameObject skipButton;
    public CanvasGroup skipButtonCanvasGroup;
    public SceneTransition sceneTransition;

    [Header("Settings")]
    public float skipButtonDelay = 30f;
    public float skipButtonFadeDuration = 1f;
    public string nextSceneName = "GameScene";
    public float fadeOutDuration = 1f;

    private bool hasTriggeredEnd = false;

    void Start()
    {
        if (skipButton != null)
            skipButton.SetActive(false);

        if (skipButtonCanvasGroup != null)
            skipButtonCanvasGroup.alpha = 0f;

        if (sceneTransition != null)
            sceneTransition.FadeInOnStart();

        if (storyVideoPlayer != null)
        {
            storyVideoPlayer.loopPointReached += OnVideoEnd;
            storyVideoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("[StoryManager] VideoPlayer belum di-assign di Inspector.");
        }

        StartCoroutine(ShowSkipButtonAfterDelay());
    }

    private IEnumerator ShowSkipButtonAfterDelay()
    {
        yield return new WaitForSeconds(skipButtonDelay);

        if (skipButton != null)
            skipButton.SetActive(true);

        if (skipButtonCanvasGroup != null)
            yield return StartCoroutine(FadeCanvasGroup(skipButtonCanvasGroup, 0f, 1f, skipButtonFadeDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        cg.alpha = from;
        cg.interactable = true;
        cg.blocksRaycasts = true;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (hasTriggeredEnd) return;
        hasTriggeredEnd = true;
        GoToGameScene();
    }

    public void OnSkip()
    {
        if (hasTriggeredEnd) return;
        hasTriggeredEnd = true;

        if (storyVideoPlayer != null)
            storyVideoPlayer.Stop();

        GoToGameScene();
    }

    private void GoToGameScene()
    {
        if (sceneTransition != null)
            sceneTransition.StartFadeAndLoad(nextSceneName, fadeOutDuration);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        if (storyVideoPlayer != null)
            storyVideoPlayer.loopPointReached -= OnVideoEnd;
    }
}
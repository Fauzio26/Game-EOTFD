using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Blackout Panel")]
    public CanvasGroup blackoutPanel;

    [Header("Default Settings")]
    public float defaultFadeDuration = 1f;

    private bool isTransitioning = false;

    void Awake()
    {
        if (blackoutPanel == null)
            Debug.LogWarning("[SceneTransition] BlackoutPanel belum di-assign di Inspector.");
    }

    public void StartFadeAndLoad(string sceneName, float duration = -1f, float holdDuration = 0f)
    {
        if (isTransitioning) return;
        if (duration < 0f) duration = defaultFadeDuration;
        StartCoroutine(FadeAndLoad(sceneName, duration, holdDuration));
    }

    public IEnumerator FadeAndLoad(string sceneName, float duration, float holdDuration = 0f)
    {
        isTransitioning = true;

        // Fade to black (durasi tetap sesuai parameter, misal 1 detik)
        yield return StartCoroutine(Fade(0f, 1f, duration));

        // Tahan layar hitam sebentar (di sinilah SFX sempat selesai)
        if (holdDuration > 0f)
            yield return new WaitForSeconds(holdDuration);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        isTransitioning = false;
    }

    public void FadeInOnStart(float duration = -1f)
    {
        if (duration < 0f) duration = defaultFadeDuration;
        if (blackoutPanel != null)
            blackoutPanel.alpha = 1f;
        StartCoroutine(Fade(1f, 0f, duration));
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (blackoutPanel == null) yield break;

        blackoutPanel.gameObject.SetActive(true);
        blackoutPanel.blocksRaycasts = true;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            blackoutPanel.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        blackoutPanel.alpha = to;

        if (Mathf.Approximately(to, 0f))
        {
            blackoutPanel.blocksRaycasts = false;
            blackoutPanel.gameObject.SetActive(false);
        }
    }
}
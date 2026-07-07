using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string gameSceneName = "GameScene";

    [Header("Panels")]
    public GameObject settingPanel;
    public GameObject creditPanel;
    public GameObject exitPanel;
    public GameObject blurOverlay;

    [Header("Scene Transition")]
    public SceneTransition sceneTransition;
    public string storySceneName = "StoryScene";

    [Header("Start Button")]
    public float startFadeDuration = 1f;   // durasi fade to black (tetap)
    public float startHoldDuration = 2f;   // lama layar hitam ditahan (biar SFX kelar)

    // ── Main Menu Buttons ──
    public void OnStart()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayStartSound();
            AudioManager.Instance.StopMusic();
        }

        if (sceneTransition != null)
            sceneTransition.StartFadeAndLoad(storySceneName, startFadeDuration, startHoldDuration);
        else
            SceneManager.LoadScene(storySceneName);
    }

    public void OnSetting()
    {
        AudioManager.Instance?.PlayClickSound();
        blurOverlay.SetActive(true);
        settingPanel.SetActive(true);
    }

    public void OnCredit()
    {
        AudioManager.Instance?.PlayClickSound();
        blurOverlay.SetActive(true);
        creditPanel.SetActive(true);
    }

    public void OnExit()
    {
        AudioManager.Instance?.PlayClickSound();
        blurOverlay.SetActive(true);
        exitPanel.SetActive(true);
    }

    // ── Close Buttons (juga dipakai untuk No/Back) ──
    public void OnBack()
    {
        AudioManager.Instance?.PlayClickSound();
        blurOverlay.SetActive(false);
        settingPanel.SetActive(false);
        creditPanel.SetActive(false);
        exitPanel.SetActive(false);
    }

    // ── Exit Confirmation (Yes) ──
    public void OnExitConfirm()
    {
        AudioManager.Instance?.PlayClickSound();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
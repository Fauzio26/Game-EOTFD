using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip defaultBGM;
    public AudioClip bossBGM;

    [Header("SFX Clips")]
    public AudioClip buttonClickClip;
    public AudioClip startButtonClip;
    public AudioClip sfxPreviewClip;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float defaultBGMVolume = 1f;
    [Range(0f, 1f)] public float bossBGMVolume     = 0.5f;

    [Header("SFX Preview Settings")]
    public float sfxPreviewCooldown = 0.15f;
    private float lastPreviewTime   = -999f;

    // ── Singleton ──
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // FIX: daftar ke event sceneLoaded supaya musik otomatis
            // berganti setiap kali scene baru dimuat (bukan cuma sekali di Start)
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // FIX: hanya unsubscribe kalau ini instance yang aktif (bukan duplikat)
        if (Instance == this)
            SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    void Start()
    {
        // Setup slider music
        if (musicSlider != null)
        {
            musicSlider.value = musicSource != null ? musicSource.volume : 1f;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        // Setup slider SFX
        if (sfxSlider != null)
        {
            sfxSlider.value = sfxSource != null ? sfxSource.volume : 1f;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // Play musik awal untuk scene pertama kali game dibuka
        // (event sceneLoaded tidak terpanggil untuk scene awal saat aplikasi start,
        // jadi tetap perlu logic ini untuk first-load)
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    // FIX: dipanggil otomatis setiap kali scene baru selesai dimuat
    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    // FIX: satu tempat untuk menentukan musik apa yang cocok untuk scene apa
    private void PlayMusicForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                if (mainMenuMusic != null)
                    PlayMusic(mainMenuMusic, defaultBGMVolume);
                break;

            case "SampleScene":
                if (defaultBGM != null)
                    PlayMusic(defaultBGM, defaultBGMVolume);
                break;

            // StoryScene sengaja tidak diberi musik di sini.
            // Kalau StoryScene butuh musik/SFX sendiri, tambahkan case di sini.
        }
    }

    // ── Music ──
    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (clip == null || musicSource == null) return;

        // FIX: kalau lagu sama TAPI sedang tidak playing (misal habis di-Stop),
        // tetap panggil Play() lagi. Sebelumnya cuma cek clip == clip,
        // jadi kalau musik sudah di-stop tapi clip-nya sama, dia tidak play ulang.
        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            musicSource.volume = volume;
            return;
        }

        musicSource.clip   = clip;
        musicSource.volume = volume;
        musicSource.loop   = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void SetMusicVolume(float value)
    {
        if (musicSource != null)
            musicSource.volume = value;
    }

    // Dipanggil dari Enemy/BossScript saat masuk area boss
    public void ChangeToBossMusic()
    {
        PlayMusic(bossBGM, bossBGMVolume);
    }

    // Dipanggil saat balik dari boss ke area normal
    public void ChangeToDefaultMusic()
    {
        PlayMusic(defaultBGM, defaultBGMVolume);
    }

    // ── SFX ──
    public void SetSFXVolume(float value)
    {
        if (sfxSource != null)
            sfxSource.volume = value;

        // Preview suara SFX saat slider digeser
        if (Time.unscaledTime - lastPreviewTime >= sfxPreviewCooldown)
        {
            lastPreviewTime = Time.unscaledTime;
            PlaySFXPreview();
        }
    }

    public void PlaySFXPreview()
    {
        if (sfxPreviewClip != null && sfxSource != null)
            sfxSource.PlayOneShot(sfxPreviewClip, sfxSource.volume);
    }

    public void PlayClickSound()
    {
        if (buttonClickClip != null && sfxSource != null)
            sfxSource.PlayOneShot(buttonClickClip, sfxSource.volume);
    }

    public void PlayStartSound()
    {
        if (startButtonClip != null && sfxSource != null)
            sfxSource.PlayOneShot(startButtonClip, sfxSource.volume);
    }
}

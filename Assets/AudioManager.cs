using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; 

    [Header("Audio Source")]
    public AudioSource bgmSource;

    [Header("Audio Clips")]
    public AudioClip defaultBGM; // Untuk backgroundmusic_1.mp3
    public AudioClip bossBGM;    // Untuk backgroundmusic_2.mp3

    // Menambahkan slider volume di Inspector (Rentang 0.0 sampai 1.0)
    [Header("Volume Settings")]
    [Range(0f, 1f)] public float defaultBGMVolume = 1f; 
    [Range(0f, 1f)] public float bossBGMVolume = 0.5f;   

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Memutar musik awal dengan volume yang sudah ditentukan
        PlayMusic(defaultBGM, defaultBGMVolume);
    }

    public void ChangeToBossMusic()
    {
        // Memutar musik bos dengan volume yang sudah ditentukan
        PlayMusic(bossBGM, bossBGMVolume);
    }

    // Mengubah fungsi PlayMusic agar menerima parameter volume
    private void PlayMusic(AudioClip clip, float volume)
    {
        // Jika lagunya sama, kita hanya perbarui volumenya saja (jika kamu mengubahnya saat game berjalan)
        if (bgmSource.clip == clip) 
        {
            bgmSource.volume = volume;
            return; 
        }

        bgmSource.clip = clip;
        bgmSource.volume = volume; // Mengatur volume sesuai lagu yang diputar
        bgmSource.loop = true; 
        bgmSource.Play();
    }
}
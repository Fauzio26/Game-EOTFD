using UnityEngine;

public class BossSFX : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private AudioClip attack1Clip;
    [SerializeField] private AudioClip attack2Clip;
    [SerializeField] private float attackVolume = 0.7f;

    [Header("Hit")]
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private float hitVolume = 0.8f;

    [Header("Death")]
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private float deathVolume = 1f;

    [Header("Footstep")]
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private float footstepVolume = 0.5f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAttack1()
    {
        if (attack1Clip == null) return;
        if (audioSource == null) return;
        audioSource.PlayOneShot(attack1Clip, attackVolume);
    }

    public void PlayAttack2()
    {
        // Kalau attack2Clip tidak diisi, fallback ke attack1Clip
        AudioClip clip = attack2Clip != null ? attack2Clip : attack1Clip;
        if (clip == null) return;
        if (audioSource == null) return;
        audioSource.PlayOneShot(clip, attackVolume);
    }

    public void PlayHit()
    {
        if (hitClip == null) return;
        if (audioSource == null) return;
        audioSource.PlayOneShot(hitClip, hitVolume);
    }

    public void PlayDeath()
    {
        if (deathClip == null) return;
        // Pakai AudioManager agar tidak terpotong saat Boss di-disable
        GameObject audioManager = GameObject.Find("AudioManager");
        if (audioManager != null)
        {
            AudioSource persistentAS = audioManager.GetComponent<AudioSource>();
            if (persistentAS != null)
                persistentAS.PlayOneShot(deathClip, deathVolume);
        }
    }

    public void PlayFootstep()
    {
        if (footstepClip == null) return;
        if (audioSource == null) return;
        audioSource.PlayOneShot(footstepClip, footstepVolume);
    }
}
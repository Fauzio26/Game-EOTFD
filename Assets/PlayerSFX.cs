using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [Header("Footstep")]
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private float footstepVolume = 0.5f;

    [Header("Attack")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private float attackVolume = 0.7f;

    [Header("Hit")]
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private float hitVolume = 0.8f;

    [Header("Jump")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip doubleJumpClip;
    [SerializeField] private float jumpVolume = 0.6f;

    [Header("Landing")]
    [SerializeField] private AudioClip landingClip;
    [SerializeField] private float landingVolume = 0.7f;

    [Header("Death")]
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip fallDeathClip;
    [SerializeField] private float deathVolume = 1f;

    // Getter untuk PlayerHealth
    public AudioClip DeathClip => deathClip;
    public AudioClip FallDeathClip => fallDeathClip;

    private AudioSource audioSource;
    private Animator animator;

    // ── TAMBAHAN: AudioSource permanen untuk suara death ──
    private static AudioSource persistentAudioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        // Cari AudioManager di scene
        if (persistentAudioSource == null)
        {
            GameObject audioManager = GameObject.Find("AudioManager");
            if (audioManager != null)
                persistentAudioSource = audioManager.GetComponent<AudioSource>();
        }
    }

    public void PlayFootstep()
    {
        if (footstepClip == null) return;
        if (audioSource == null) return;
        audioSource.PlayOneShot(footstepClip, footstepVolume);
    }

    public void PlayAttack()
    {
        if (attackClip == null) return;
        if (audioSource == null) return;
        audioSource.PlayOneShot(attackClip, attackVolume);
    }

    public void PlayHit()
    {
        if (hitClip == null) return;
        if (audioSource == null) return;
        audioSource.PlayOneShot(hitClip, hitVolume);
    }

    public void PlayJump()
    {
        if (jumpClip == null) return;
        if (audioSource == null) return;
        audioSource.PlayOneShot(jumpClip, jumpVolume);
    }

    public void PlayDoubleJump()
    {
        AudioClip clip = doubleJumpClip != null ? doubleJumpClip : jumpClip;
        if (clip == null) return;
        if (audioSource == null) return;
        audioSource.PlayOneShot(clip, jumpVolume);
    }

    public void PlayLanding()
    {
        if (landingClip == null) return;
        if (audioSource == null) return;
        audioSource.PlayOneShot(landingClip, landingVolume);
    }

    public void PlayDeath()
    {
        if (deathClip == null) return;
        // Pakai persistentAudioSource agar tidak terpotong saat SetActive(false)
        if (persistentAudioSource != null)
            persistentAudioSource.PlayOneShot(deathClip, deathVolume);
    }

    public void PlayFallDeath()
    {
        AudioClip clip = fallDeathClip != null ? fallDeathClip : deathClip;
        if (clip == null) return;
        if (persistentAudioSource != null)
            persistentAudioSource.PlayOneShot(clip, deathVolume);
    }
}
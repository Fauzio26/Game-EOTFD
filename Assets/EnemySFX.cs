using UnityEngine;

public class EnemySFX : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private float attackVolume = 0.7f;

    [Header("Hit")]
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private float hitVolume = 0.8f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
}
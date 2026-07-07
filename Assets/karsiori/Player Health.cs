using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHP = 3;

    [Header("Invincibility & Blink")]
    [Tooltip("Durasi tidak bisa kena damage setelah terkena duri (detik)")]
    [SerializeField] private float invincibleDuration = 1.5f;
    [Tooltip("Kecepatan kedap-kedip — makin kecil makin cepat")]
    [SerializeField] private float blinkInterval = 0.08f;

    [Header("Knockback")]
    [Tooltip("Durasi input player diblokir saat kena knockback (detik)")]
    [SerializeField] private float knockbackDuration = 0.25f;

    [Header("Death")]
    [Tooltip("Sesuaikan dengan durasi animasi mati player")]
    [SerializeField] private float deathDelay = 1.5f;

    public bool IsInvincible  { get; private set; }
    public bool IsKnockedBack { get; private set; }
    public int  CurrentHP     { get; private set; }

    public int GetMaxHP() => maxHP;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private PlayerSFX playerSFX;
    private PlayerBlock playerBlock; // ← TAMBAHAN

    private void Awake()
    {
        rb          = GetComponent<Rigidbody2D>();
        sr          = GetComponent<SpriteRenderer>();
        animator    = GetComponent<Animator>();
        playerSFX   = GetComponent<PlayerSFX>();
        playerBlock = GetComponent<PlayerBlock>(); // ← TAMBAHAN
        CurrentHP   = maxHP;
    }

    public void TakeDamage(int damage, Vector2 knockbackForce)
    {
        if (IsInvincible) return;

        // ── TAMBAHAN: cek block/parry ──
        if (playerBlock != null && playerBlock.TryParry())
            return; // damage diblock, tidak ada efek apapun

        CurrentHP = Mathf.Max(CurrentHP - damage, 0);
        Debug.Log($"[PlayerHealth] HP: {CurrentHP}/{maxHP}");

        if (playerSFX != null) playerSFX.PlayHit();

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(knockbackForce, ForceMode2D.Impulse);

        if (CurrentHP <= 0)
        {
            Die();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(InvincibilityRoutine());
        StartCoroutine(KnockbackRoutine());
    }

    public void ResetHP()
    {
        CurrentHP     = maxHP;
        IsInvincible  = false;
        IsKnockedBack = false;
        StopAllCoroutines();
        if (sr != null) sr.enabled = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (animator != null)
        {
            animator.ResetTrigger("isDead");
            animator.SetBool("isBlocking", false); // ← TAMBAHAN
            animator.SetBool("isRunning", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }
        Debug.Log("[PlayerHealth] HP di-reset.");
    }

    public void InstantKill()
    {
        if (IsInvincible) return;

        CurrentHP = 0;
        Debug.Log("[PlayerHealth] Player jatuh ke lubang!");
        StopAllCoroutines();

        if (playerSFX != null) playerSFX.PlayFallDeath();
        gameObject.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.RespawnPlayer();
    }

    private IEnumerator InvincibilityRoutine()
    {
        IsInvincible = true;

        float elapsed = 0f;
        while (elapsed < invincibleDuration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        sr.enabled   = true;
        IsInvincible = false;
    }

    private IEnumerator KnockbackRoutine()
    {
        IsKnockedBack = true;
        yield return new WaitForSeconds(knockbackDuration);
        IsKnockedBack = false;
    }

    private void Die()
    {
        Debug.Log("[PlayerHealth] Player mati!");

        IsKnockedBack = true;
        IsInvincible  = true;

        if (playerSFX != null) playerSFX.PlayDeath();

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetBool("isBlocking", false); // ← TAMBAHAN
            animator.SetBool("isRunning", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            animator.SetTrigger("isDead");
        }

        StopAllCoroutines();
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(0.3f);

        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        yield return new WaitForSeconds(deathDelay - 0.3f);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        gameObject.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.RespawnPlayer();
    }
}
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings Boss")]
    [SerializeField] private float startingHealth = 200f;
    private float currentHealth;

    [Header("UI Floating Health Bar")]
    // Referensi ke skrip floating health bar yang baru dibuat 
    [SerializeField] private FloatingHealthBar floatingHealthBar;

    private Animator anim;
    private bool isDead = false;
    private BossSFX bossSFX;
    private BossMovement bossMovement;

    [Header("Components to Disable on Death")]
    [SerializeField] private Behaviour[] componentsToDisable;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = startingHealth;
        bossSFX = GetComponent<BossSFX>();
        bossMovement = GetComponent<BossMovement>();
    }

        private void Start()
    {
        // Beri tahu nilai darah penuh saat game dimulai
        if (floatingHealthBar != null)
        {
            floatingHealthBar.UpdateHealthBar(currentHealth, startingHealth);
        }
    }


    public void TakeDamage(float _damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        // Setiap kali boss terluka, perbarui slidernya
        if (floatingHealthBar != null)
        {
            floatingHealthBar.UpdateHealthBar(currentHealth, startingHealth);
        }

        if (currentHealth > 0)
        {
            if (anim != null) anim.SetTrigger("Hit");
            if (bossSFX != null) bossSFX.PlayHit();
            if (bossMovement != null) bossMovement.ForceAttack();
            
            Debug.Log(gameObject.name + " (BOSS) Terluka. Sisa Darah: " + currentHealth);
        }
        else
        {
            if (!isDead) Dead();
        }
    }

    private void Dead()
    {
        isDead = true;

        if (anim != null) anim.SetTrigger("Dead");
        if (bossSFX != null) bossSFX.PlayDeath();

        foreach (Behaviour component in componentsToDisable)
        {
            if (component != null) component.enabled = false;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Sembunyikan health bar saat Boss hancur/mati
        if (floatingHealthBar != null)
        {
            floatingHealthBar.gameObject.SetActive(false);
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
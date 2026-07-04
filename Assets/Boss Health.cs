using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings Boss")]
    [SerializeField] private float startingHealth = 200f;
    private float currentHealth;

    [Header("Health Bar")]
    [SerializeField] private Slider healthBarSlider;

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

        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = startingHealth;
            healthBarSlider.value    = startingHealth;
        }
    }

    public void TakeDamage(float _damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (healthBarSlider != null)
            healthBarSlider.value = currentHealth;

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
        if (bossSFX != null) bossSFX.PlayDeath(); // ← TAMBAHAN

        if (healthBarSlider != null)
            healthBarSlider.gameObject.SetActive(false);

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
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
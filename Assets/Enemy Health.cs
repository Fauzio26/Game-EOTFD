using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float startingHealth = 5f;
    private float currentHealth;

    private Animator anim;
    private bool isDead = false;
    private EnemySFX enemySFX; // ← TAMBAHAN

    [Header("Components to Disable on Death")]
    [SerializeField] private Behaviour[] componentsToDisable;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = startingHealth;
        enemySFX = GetComponent<EnemySFX>(); // ← TAMBAHAN
    }

    public void TakeDamage(float _damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            if (anim != null) anim.SetTrigger("Hit");
            if (enemySFX != null) enemySFX.PlayHit(); // ← TAMBAHAN
            Debug.Log(gameObject.name + " Terluka. Sisa Darah: " + currentHealth);
        }
        else
        {
            if (!isDead)
            {
                Dead();
            }
        }
    }

    private void Dead()
    {
        isDead = true;

        if (anim != null) anim.SetTrigger("Dead");

        foreach (Behaviour component in componentsToDisable)
        {
            if (component != null) component.enabled = false;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
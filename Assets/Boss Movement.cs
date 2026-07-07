using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Pengaturan Patroli (Batas Gerak)")]
    public Transform leftBoundary;
    public Transform rightBoundary;
    public float moveSpeed = 2f;
    private bool MovingRight = true;

    [Header("Pengaturan Serangan")]
    public Transform player;
    public float chaseRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    private float nextAttackTime = 0f;
    public int damageAmount = 3;

    [Header("Counter Attack")]
    [SerializeField] private float counterAttackCooldown = 0.5f;
    private float lastCounterAttackTime = -999f;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector3 initScale;
    private BossSFX bossSFX; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        initScale = transform.localScale;
        bossSFX = GetComponent<BossSFX>(); 

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

     void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (anim != null) anim.SetBool("Moving", false);

            if (Time.time >= nextAttackTime)
            {
                if (anim != null) anim.SetTrigger("Attack");
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (leftBoundary == null || rightBoundary == null)
        {
            Debug.LogError("[Boss] leftBoundary atau rightBoundary NULL!");
            return;
        }

        if (anim != null) anim.SetBool("Moving", true);

        if (MovingRight)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            transform.localScale = new Vector3(Mathf.Abs(initScale.x), initScale.y, initScale.z);

            if (transform.position.x >= rightBoundary.position.x)
                MovingRight = false;
        }
        else
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            transform.localScale = new Vector3(-Mathf.Abs(initScale.x), initScale.y, initScale.z);

            if (transform.position.x <= leftBoundary.position.x)
                MovingRight = true;
        }
    }

    void ChasePlayer()
    {
        if (anim != null) anim.SetBool("Moving", true);

        if (player.position.x > transform.position.x)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            transform.localScale = new Vector3(Mathf.Abs(initScale.x), initScale.y, initScale.z);
            MovingRight = true;
        }
        else
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            transform.localScale = new Vector3(-Mathf.Abs(initScale.x), initScale.y, initScale.z);
            MovingRight = false;
        }
    }

    public void DamagePlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth == null) return;

            float dirX = player.position.x >= transform.position.x ? 1f : -1f;
            Vector2 knockback = new Vector2(dirX * 3f, 5f);

            playerHealth.TakeDamage(damageAmount, knockback); // ← ganti dari 1 ke damageAmount
            Debug.Log("Player terkena tebasan Boss.");
        }
    }

    // Dipanggil via Animation Event di clip Attack ← TAMBAHAN
    public void PlayAttackSFX()
    {
        if (bossSFX != null) bossSFX.PlayAttack1();
    }

    // Dipanggil via Animation Event di clip Movement ← TAMBAHAN
    public void PlayFootstepSFX()
    {
        if (bossSFX != null) bossSFX.PlayFootstep();
    }

    public void ForceAttack()
    {
        // Cegah spam counter-attack kalau player hit berkali-kali sangat cepat
        if (Time.time < lastCounterAttackTime + counterAttackCooldown) return;

        if (anim != null) anim.SetTrigger("Attack");
        nextAttackTime = Time.time + attackCooldown; // reset cooldown attack normal juga
        lastCounterAttackTime = Time.time;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
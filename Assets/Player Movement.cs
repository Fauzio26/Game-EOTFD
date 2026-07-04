using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Pengaturan Serangan Hero")]
    public Transform titikSerangkanan;
    public Transform titikSerangkiri;
    public float radiusSerang = 0.6f;
    public LayerMask layerMusuh;
    public float damageSerang = 20f;

    private Vector2 movement;
    private Vector2 screenBounds;
    private float playerHalfwidth;
    private float xPostLastFrame;

    // Referensi ke PlayerHealth untuk blokir movement saat knockback
    private PlayerHealth playerHealth;

    private void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(
            new Vector2(Screen.width, Screen.height));

        playerHalfwidth = spriteRenderer.bounds.extents.x;
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        // Blokir semua input saat kena knockback
        if (playerHealth != null && playerHealth.IsKnockedBack)
            return;

        Handlemovement();
        // ClampMovement();
        FlipCharacterX();
        HandleAttack();
    }

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (animator != null)
            {
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Attack");
                animator.CrossFade("Attack", 0f);
            }
        }
    }

    // Dipanggil melalui Animation Event pada clip animasi menyerang
    public void BerikanDamageKeMusuh()
    {
        if (titikSerangkanan == null || titikSerangkiri == null)
        {
            Debug.LogError("Titik Serang Kanan atau Kiri belum dimasukkan di Inspector!");
            return;
        }

        // Jika sprite di-flipX berarti menghadap kiri, gunakan titikSerangkiri
        Transform titikAktif = spriteRenderer.flipX ? titikSerangkiri : titikSerangkanan;

        Collider2D[] musuhTerkena = Physics2D.OverlapCircleAll(titikAktif.position, radiusSerang, layerMusuh);

        foreach (Collider2D colMusuh in musuhTerkena)
        {
            EnemyHealth enemyHealth = colMusuh.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageSerang);
            }
            BossHealth bossHealth = colMusuh.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damageSerang);
            }
        }
    }

    private void FlipCharacterX()
    {
        float input = Input.GetAxis("Horizontal");

        if (input > 0 && transform.position.x > xPostLastFrame)
            spriteRenderer.flipX = false;
        else if (input < 0 && transform.position.x < xPostLastFrame)
            spriteRenderer.flipX = true;

        xPostLastFrame = transform.position.x;
    }

    private void Handlemovement()
    {
        float input = Input.GetAxis("Horizontal");

        movement.x = input * speed * Time.deltaTime;
        transform.Translate(movement);

        if (animator == null) return;

        // Ambil status animasi dari PlayerJump.cs
        bool isJumping = animator.GetBool("isJumping");
        bool isFalling = animator.GetBool("isFalling");

        // Run hanya saat tidak Jump dan tidak Fall
        if (input != 0 && !isJumping && !isFalling)
            animator.SetBool("isRunning", true);
        else
            animator.SetBool("isRunning", false);
    }

    private void ClampMovement()
    {
        float clampedX = Mathf.Clamp(
            transform.position.x,
            -screenBounds.x + playerHalfwidth,
            screenBounds.x - playerHalfwidth);

        Vector2 pos = transform.position;
        pos.x = clampedX;
        transform.position = pos;
    }

    // Menggambar lingkaran di Scene view untuk memantau jangkauan serangan
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (titikSerangkanan != null) Gizmos.DrawWireSphere(titikSerangkanan.position, radiusSerang);
        if (titikSerangkiri != null) Gizmos.DrawWireSphere(titikSerangkiri.position, radiusSerang);
    }
}
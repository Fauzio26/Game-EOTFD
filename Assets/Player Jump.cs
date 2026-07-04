using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [SerializeField] private float jumpforce = 6f;
    [SerializeField] private float doubleJumpforce = 6f;

    private float playerHalfHeight;
    private bool canDoubleJump;

    // Referensi ke PlayerHealth untuk blokir jump saat knockback
    private PlayerHealth playerHealth;
    private PlayerSFX playerSFX; // ← TAMBAHAN
    private bool wasGrounded = true; // ← TAMBAHAN — track status grounded frame sebelumnya

private void Start()
{
    playerHalfHeight = spriteRenderer.bounds.extents.y;

    if (animator == null)
        animator = GetComponent<Animator>();

    playerHealth = GetComponent<PlayerHealth>();
    playerSFX = GetComponent<PlayerSFX>();
}

private void Update()
{
    bool grounded = GetIsGrounded();

    if (playerHealth != null && playerHealth.IsKnockedBack)
    {
        UpdateAnimations(grounded);
        return;
    }

    // Deteksi landing — baru menyentuh ground setelah di udara
    if (grounded && !wasGrounded)
    {
        if (playerSFX != null) playerSFX.PlayLanding(); // ← TAMBAHAN
    }

    // Jump pertama
    if (Input.GetButtonDown("Jump") && grounded)
    {
        Jump(jumpforce);
        if (playerSFX != null) playerSFX.PlayJump();
    }
    // Double Jump
    else if (Input.GetButtonDown("Jump") && !grounded && canDoubleJump)
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        Jump(doubleJumpforce);
        canDoubleJump = false;
        if (playerSFX != null) playerSFX.PlayDoubleJump();
    }

    UpdateAnimations(grounded);

    wasGrounded = grounded; // ← TAMBAHAN — simpan status grounded frame ini
}

    // Dipisah ke fungsi sendiri agar tetap jalan saat knockback
    private void UpdateAnimations(bool grounded)
    {
        float yVel = rigidBody.velocity.y;

        if (grounded)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }
        else
        {
            // Angka disesuaikan agar lebih responsif mendeteksi naik/turun
            if (yVel > 0.05f)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isFalling", false);
            }
            else if (yVel < -0.05f)
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
            }
        }
    }

    private bool GetIsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            playerHalfHeight + 0.05f, // Jarak raycast diperpendek agar deteksi tanah akurat
            LayerMask.GetMask("Ground")
        );

        if (hit.collider != null)
        {
            canDoubleJump = true;
            return true;
        }

        return false;
    }

    private void Jump(float force)
    {
        rigidBody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        if (spriteRenderer == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.down * (spriteRenderer.bounds.extents.y + 0.05f) // Gizmos disesuaikan dengan raycast baru
        );
    }
}
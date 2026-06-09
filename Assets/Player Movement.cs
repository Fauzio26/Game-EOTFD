using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private Vector2 movement;

    private Vector2 screenBounds;

    private float playerHalfwidth;
    private float xPostLastFrame;

    private void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(
            new Vector2(Screen.width, Screen.height));

        playerHalfwidth = spriteRenderer.bounds.extents.x;
    }

    private void Update()
    {
        Handlemovement();
        //ClampMovement();
        FlipCharacterX();
        HandleAttack();
    }

        private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
            animator.CrossFade("Attack", 0f);
        }
    }

    private void FlipCharacterX()
    {
        float input = Input.GetAxis("Horizontal");

        if (input > 0 &&
            transform.position.x > xPostLastFrame)
        {
            spriteRenderer.flipX = false;
        }
        else if (input < 0 &&
                 transform.position.x < xPostLastFrame)
        {
            spriteRenderer.flipX = true;
        }

        xPostLastFrame = transform.position.x;
    }

    private void Handlemovement()
    {
        float input = Input.GetAxis("Horizontal");

        movement.x = input * speed * Time.deltaTime;

        transform.Translate(movement);

        // Ambil status animasi dari PlayerJump.cs
        bool isJumping = animator.GetBool("isJumping");
        bool isFalling = animator.GetBool("isFalling");

        // Run hanya saat tidak Jump dan tidak Fall
        if (input != 0 && !isJumping && !isFalling)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
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
}
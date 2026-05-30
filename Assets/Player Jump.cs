using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private float jumpforce = 6;
    [SerializeField] private float doubleJumpforce = 6f;

    private bool isGrounded;

    private float playerHalfHeight;
    
    private bool canDoubleJump;

    // Update is called once per frame
    private void Start()
    {
        playerHalfHeight = spriteRenderer.bounds.extents.y;
    }
    void Update()
    {
         
        if (Input.GetButtonDown("Jump") && GetIsGrounded()){
            Jump(jumpforce);
        }
        else if (Input.GetButtonDown("Jump") && !GetIsGrounded() && canDoubleJump){
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0;
            Jump(doubleJumpforce);
            canDoubleJump = false;
        }
    }

    private void onCollisionEnter2D(Collision2D other)
    {
        GetIsGrounded();
    }

    private bool GetIsGrounded(){
        bool hit = Physics2D.Raycast(transform.position, Vector2.down, playerHalfHeight + 0.1f, LayerMask.GetMask("Ground"));
        if (hit){
            canDoubleJump = true;
        }
        return hit;
    }
    private void Jump(float force){
        rigidBody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }
}

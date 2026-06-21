using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    // [SerializeField] private Animator animator;
    [SerializeField] private float speed = 3f;
    [SerializeField] private int startDirection = 1;
    [SerializeField] private bool stayOnLedges = true;

    private int currentDirection;
    private float halfWidht;
    private float halfHeight;
    private Vector2 movement;
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
       halfWidht = spriteRenderer.bounds.extents.x;
       halfHeight = spriteRenderer.bounds.extents.y;
       currentDirection = startDirection; 
       spriteRenderer.flipX = startDirection == 1 ? false : true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movement.x = speed * currentDirection;
        movement.y = rigidBody.velocity.y;
        rigidBody.velocity = movement;
        setDirection();
    }

    // PERBAIKAN: Huruf 'O' besar
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground")){
            isGrounded = true;
        }
        else{
            isGrounded = false;
        }
    }

    // PERBAIKAN: Diubah menjadi Exit agar tidak duplikat dengan fungsi di atasnya
    private void OnCollisionExit2D(Collision2D other)
    {
        isGrounded = false;
    }

    private void setDirection()
    {
        if (!isGrounded) return;

        Vector2 rightPos = transform.position;
        Vector2 leftPos = transform.position;
        rightPos.x += halfWidht;
        leftPos.x -= halfWidht;

        if (rigidBody.velocity.x > 0 )
        {
            if (Physics2D.Raycast(transform.position, Vector2.right, halfWidht + 0.1f, LayerMask.GetMask("Ground")))
            {
                currentDirection *= -1;
                spriteRenderer.flipX = true;
            }
            // PERBAIKAN: 0,1f menjadi 0.1f dan hapus titik dua (:) sebelum LayerMask
            else if (stayOnLedges && !Physics2D.Raycast(rightPos, Vector2.down, halfHeight + 0.1f, LayerMask.GetMask("Ground")))
            {
                currentDirection *= -1;
                spriteRenderer.flipX = true;
            }    
        }
        else if (rigidBody.velocity.x < 0)
        {
            if (Physics2D.Raycast(transform.position, Vector2.left, halfWidht + 0.1f, LayerMask.GetMask("Ground")))
            {
                currentDirection *= -1;
                spriteRenderer.flipX = false;
             }
             // PERBAIKAN: 0,1f menjadi 0.1f dan hapus titik dua (:) sebelum LayerMask
             else if (stayOnLedges && !Physics2D.Raycast(leftPos, Vector2.down, halfHeight + 0.1f, LayerMask.GetMask("Ground")))
             {
                currentDirection *= -1;
                spriteRenderer.flipX = false;
             } 
        }
    }
}
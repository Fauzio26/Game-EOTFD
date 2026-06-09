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

    private int currentDirection;
    private float halfWidht;
    private Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
       halfWidht = spriteRenderer.bounds.extents.x;
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

    private void setDirection()
    {
        if (Physics2D.Raycast(transform.position, Vector2.right, halfWidht + 0.1f, LayerMask.GetMask("Ground")) && rigidBody.velocity.x > 0){
            currentDirection *= -1;
            spriteRenderer.flipX = true;
        }
        else if (Physics2D.Raycast(transform.position, Vector2.left, halfWidht + 0.1f, LayerMask.GetMask("Ground")) && rigidBody.velocity.x < 0){
            currentDirection *= -1;
            spriteRenderer.flipX = false;
        }
    }
}

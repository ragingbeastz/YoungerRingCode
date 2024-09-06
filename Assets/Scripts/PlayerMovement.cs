using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int speed = 10;
    public float jumpAmount = 5;
    private Rigidbody2D characterBody;
    private Vector2 velocity;
    private Vector2 inputMovement;
    public Animator animator;

    // Ground check variable
    private bool isGrounded;
    private bool isLookingRight = true;
    private string groundLayer = "Floor"; //Set Ground Layer
    private float yVelocity;

    void Start()
    {
        velocity = new Vector2(speed, speed);
        characterBody = GetComponent<Rigidbody2D>();
        
    }

    void Update()
    {
        yVelocity = characterBody.velocity.y;
        animator.SetFloat("yVelocity",yVelocity);

        if (!isLookingRight){
            GetComponent<SpriteRenderer>().flipX = true;
        }

        else if (isLookingRight){
            GetComponent<SpriteRenderer>().flipX = false;
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Space key pressed: Jump amount: " + jumpAmount);
            characterBody.AddForce(Vector2.up * jumpAmount, ForceMode2D.Impulse);
        }
    
        // Moving Left
        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("A key held");
            isLookingRight = false;
            animator.SetFloat("isMoving",1);
            characterBody.velocity = new Vector2(-speed, characterBody.velocity.y); // Move left
        }
    
        // Moving Right
        if (Input.GetKey(KeyCode.D))
        {
            Debug.Log("D key held");
            isLookingRight = true;
            animator.SetFloat("isMoving",1);
            characterBody.velocity = new Vector2(speed, characterBody.velocity.y); // Move right
        }

        // Stop horizontal movement if neither A nor D is pressed
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            animator.SetFloat("isMoving",0);
            characterBody.velocity = new Vector2(0, characterBody.velocity.y); // Stop horizontal movement
        }
    
        inputMovement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            0
        );
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collides with the ground layer
        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayer))
        {
            isGrounded = true;
            animator.SetFloat("isJumping",0);
            Debug.Log("Player is grounded");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player exits collision with the ground layer
        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayer))
        {
            isGrounded = false;
            animator.SetFloat("isJumping",1);
            Debug.Log("Player is not grounded");
        }
    }
}


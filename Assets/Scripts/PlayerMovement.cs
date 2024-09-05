using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int speed = 20;
    public float jumpAmount = 10;
    private Rigidbody2D characterBody;
    private Vector2 velocity;
    private Vector2 inputMovement;

    // Ground check variable
    private bool isGrounded;
    private string groundLayer = "Floor"; //Set Ground Layer

    void Start()
    {
        velocity = new Vector2(speed, speed);
        characterBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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
            characterBody.velocity = new Vector2(-speed, characterBody.velocity.y); // Move left
        }
    
        // Moving Right
        if (Input.GetKey(KeyCode.D))
        {
            Debug.Log("D key held");
            characterBody.velocity = new Vector2(speed, characterBody.velocity.y); // Move right
        }
    
        // Stop horizontal movement if neither A nor D is pressed
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
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
            Debug.Log("Player is grounded");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player exits collision with the ground layer
        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayer))
        {
            isGrounded = false;
            Debug.Log("Player is not grounded");
        }
    }
}


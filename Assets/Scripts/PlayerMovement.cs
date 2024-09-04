using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Speed of the player
    public int speed = 20;
    
    // Reference to the Rigidbody2D component
    private Rigidbody2D characterBody;
    
    // Velocity vector based on speed
    private Vector2 velocity;
    
    // Vector to store input movement
    private Vector2 inputMovement;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize velocity with the speed value
        velocity = new Vector2(speed, speed);
        
        // Get the Rigidbody2D component attached to the GameObject
        characterBody = GetComponent<Rigidbody2D>();   
    }

    // Update is called once per frame
    void Update()
    {
        // Get raw input from the horizontal and vertical axes
        inputMovement = new Vector2 (
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
    }

    // FixedUpdate is called at a fixed interval and is used for physics calculations
    private void FixedUpdate()
    {
        // Calculate the change in position based on input, velocity, and time
        Vector2 delta = inputMovement * velocity * Time.deltaTime;
        
        // Calculate the new position
        Vector2 newPosition = characterBody.position + delta;
        
        // Move the Rigidbody2D to the new position
        characterBody.MovePosition(newPosition);
    }
}
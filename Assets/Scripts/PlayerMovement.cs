using UnityEngine;
using System.Collections;
using System;

public class PlayerMovement : MonoBehaviour
{
    public int speed = 8;
    public float jumpAmount = 10;
    public float dodgeAmount = 5;
    public float rollDuration = 1f; // Duration to wait before setting isRolling to 0
    public float doubleTapTime = 0.3f; 

    private Rigidbody2D characterBody;
    private Vector2 velocity;
    private Vector2 inputMovement;
    public Animator animator;

    // Ground check variable
    private bool isGrounded;
    private bool isLookingRight = true;
    private bool isRolling = false;
    private bool isAttack1;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
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
        animator.SetFloat("yVelocity", yVelocity);

        if (!isLookingRight)
        {
            transform.Find("Player").GetComponent<SpriteRenderer>().flipX = true;
        }

        else if (isLookingRight)
        {
            transform.Find("Player").GetComponent<SpriteRenderer>().flipX = false;
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            characterBody.AddForce(Vector2.up * jumpAmount, ForceMode2D.Impulse);
        }

        //Dodging
        if (Input.GetKeyDown(KeyCode.J) && !isRolling && isGrounded) // Check if not already rolling
        {
            Roll();
        }

        //Attacking
        if (Input.GetKeyDown(KeyCode.K) && !(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !isRolling) // Check if not rolling
        {
            float currentAttackTime = Time.time;
            float difference = currentAttackTime-lastAttackTime;
            if (difference>=0.4f){
                isAttack1 = true;
            }

            if (isAttack1){
                StartCoroutine(AllowForAnimation("isAttacking1", 0.4f));
            }

            else{
                StartCoroutine(AllowForAnimation("isAttacking2", 0.4f));
            }

            isAttack1 = !isAttack1;
            lastAttackTime = currentAttackTime;
        }

        //Moving Left
        if (Input.GetKey(KeyCode.A) && !isRolling)
        {
            isLookingRight = false;
            animator.SetFloat("isMoving", 1);
            characterBody.velocity = new Vector2(-speed, characterBody.velocity.y); // Move left
        }

        // Moving Right
        if (Input.GetKey(KeyCode.D) && !isRolling)
        {
            isLookingRight = true;
            animator.SetFloat("isMoving", 1);
            characterBody.velocity = new Vector2(speed, characterBody.velocity.y); // Move right
        }

        if ((Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A)) && !isRolling)
        {
            animator.SetFloat("isMoving", 0);
            characterBody.velocity = new Vector2(0, characterBody.velocity.y); // Move right
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collides with the ground layer
        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayer))
        {
            isGrounded = true;
            animator.SetFloat("isJumping", 0);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player exits collision with the ground layer
        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayer))
        {
            isGrounded = false;
            animator.SetFloat("isJumping", 1);
        }
    }

    void Roll()
    {
        isRolling = true; // Set rolling flag to true

        if(animator.GetFloat("isMoving") != 0){
            characterBody.velocity = new Vector2(0, characterBody.velocity.y);
            animator.SetFloat("isMoving", 0);
        }

        if (!isRolling)
        {
            if (isLookingRight)
            {
                characterBody.AddForce(Vector2.right * dodgeAmount, ForceMode2D.Impulse);
            }
            else
            {
                characterBody.AddForce(Vector2.left * dodgeAmount, ForceMode2D.Impulse);
            }
        }

        StartCoroutine(AllowForAnimation("isRolling", 0.9f));
        isRolling = false;
    }

    IEnumerator AllowForAnimation(string animation, float duration)
    {
        animator.SetFloat(animation, 1);
        yield return new WaitForSeconds(duration);
        animator.SetFloat(animation, 0);
    }
}


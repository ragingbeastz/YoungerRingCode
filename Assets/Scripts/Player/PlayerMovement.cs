using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public int speed = 8;
    public float jumpAmount = 10;
    public float dodgeAmount = 10;
    public float rollDuration = 1f; // Duration to wait before setting isRolling to 0
    public float doubleTapTime = 0.3f; 

    private Rigidbody2D characterBody;
    private Vector2 velocity;
    private Vector2 inputMovement;
    public Animator animator;
    public UnityEngine.UI.Image staminaBar;
    public UnityEngine.UI.Image healthBar;

    // Ground check variable
    private bool isGrounded;
    private bool isLookingRight = true;
    private bool isRolling = false;
    private bool isAttack1;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private float lastHealTime = 0f;
    private float lastDodgeTime = 0f;
    private string groundLayer = "Floor"; //Set Ground Layer
    private float yVelocity;
    

    void Start()
    {
        velocity = new Vector2(speed, speed);
        characterBody = GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        //Determine if still
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("player_Idle")){
            characterBody.velocity = new Vector2(0, characterBody.velocity.y);
        }

        //Determine if attacking
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("player_Attack2") || animator.GetCurrentAnimatorStateInfo(0).IsName("player_Attack")){
            isAttacking = true;
        }

        else{
            isAttacking = false;
        }

        yVelocity = characterBody.velocity.y;
        animator.SetFloat("yVelocity", yVelocity);

        //Change direction of player
        if (!isLookingRight)
        {
            transform.Find("Character").GetComponent<SpriteRenderer>().flipX = true;
        }

        else if (isLookingRight)
        {
            transform.Find("Character").GetComponent<SpriteRenderer>().flipX = false;
        }

        // Jumping
        if (
        Input.GetKeyDown(KeyCode.Space) 
        && isGrounded 
        && animator.GetFloat("isRolling") == 0 
        && staminaBar.fillAmount >=0.2f
        )
        {
            characterBody.AddForce(Vector2.up * jumpAmount, ForceMode2D.Impulse);
            staminaBar.fillAmount -= 0.2f;
        }

        //Potion
        if (
        Input.GetKeyDown(KeyCode.L)
        && animator.GetFloat("isRolling") == 0 
        && !isAttacking
        && isGrounded
        && !(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) 
        && !animator.GetCurrentAnimatorStateInfo(0).IsName("player_Potion")
        ){
            float currentHealTime = Time.time;
            Debug.Log(currentHealTime);
            Debug.Log(lastHealTime);
            if ((currentHealTime - lastHealTime) >= 1.1f ){
                StartCoroutine(AllowForAnimation("isPotion", 0.3f));
                healthBar.fillAmount += 0.5f;
                lastHealTime = currentHealTime;   
            }
            
        } 

        //Dodging
        if (
        Input.GetKeyDown(KeyCode.J) 
        && !isAttacking 
        && animator.GetFloat("isRolling") == 0 
        && isGrounded 
        && staminaBar.fillAmount >=0.3f)
        {
            isRolling = true;
            if (isLookingRight)
            {
                characterBody.velocity = new Vector2(dodgeAmount, characterBody.velocity.y);
            }
            else
            {
                characterBody.velocity = new Vector2(-dodgeAmount, characterBody.velocity.y);
            }
            staminaBar.fillAmount -= 0.3f;
            StartCoroutine(AllowForAnimation("isRolling", 0.9f));            
            isRolling = false;  

        }

        //Attacking
        if (
        Input.GetKeyDown(KeyCode.K) 
        && !(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) 
        && animator.GetFloat("isRolling") == 0 
        && !animator.GetCurrentAnimatorStateInfo(0).IsName("player_Attack2")
        && staminaBar.fillAmount >= 0.15f
        && !isAttacking
        && isGrounded) 
        {
            float currentAttackTime = Time.time;
            float difference = currentAttackTime-lastAttackTime;
            if (difference>=0.6f){
                isAttack1 = true;
            }

            if (isAttack1 && !animator.GetCurrentAnimatorStateInfo(0).IsName("player_Attack")){
                StartCoroutine(AllowForAnimation("isAttacking1", 0.3f));
            }

            else {
                StartCoroutine(AllowForAnimation("isAttacking2", 0.3f));
            }
            
            staminaBar.fillAmount -= 0.15f;
            isAttack1 = !isAttack1;
            lastAttackTime = currentAttackTime;
        }

        //Moving Left
        if (Input.GetKey(KeyCode.A) && animator.GetFloat("isRolling") == 0)
        {
            isLookingRight = false;
            animator.SetFloat("isMoving", 1);
            characterBody.velocity = new Vector2(-speed, characterBody.velocity.y); // Move left
        }

        // Moving Right
        if (Input.GetKey(KeyCode.D) && animator.GetFloat("isRolling") == 0)
        {
            isLookingRight = true;
            animator.SetFloat("isMoving", 1);
            characterBody.velocity = new Vector2(speed, characterBody.velocity.y); // Move right
        }

        if ((Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A)))
        {
            animator.SetFloat("isMoving", 0);
            if(animator.GetFloat("isRolling") == 0){
                characterBody.velocity = new Vector2(0, characterBody.velocity.y); // Move right
            }
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

    IEnumerator AllowForAnimation(string animation, float duration)
    {
        animator.SetFloat(animation, 1);
        yield return new WaitForSeconds(duration);
        animator.SetFloat(animation, 0);
    }
}


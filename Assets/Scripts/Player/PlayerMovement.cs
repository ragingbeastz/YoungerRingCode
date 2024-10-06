using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Timeline;

public class PlayerMovement : MonoBehaviour
{
    // Player Stats
    public int speed = 8;
    public float jumpAmount = 10;
    public float dodgeAmount = 10;
    public float rollDuration = 1f;
    public float doubleTapTime = 0.3f;
    public float attackRange = 5.0f;
    public int potionsLeft = 5;
    public bool invincible = false; 

    // Components
    private Rigidbody2D characterBody;
    private Vector2 velocity;
    private Vector2 inputMovement;
    private AudioSource soundManager;
    public Animator animator;
    public UnityEngine.UI.Image staminaBar;
    public UnityEngine.UI.Image healthBar;
    public UnityEngine.UI.Image tempHealthBar;
    public SpriteRenderer spriteRenderer;
    public TextMeshProUGUI potionsLeftText;
    public UnityEngine.UI.Image Deathscreen;
    public UnityEngine.UI.Image TransitionImage;

    Color originalColor;

    //Sounds
    public AudioClip audio_Hurt;
    public AudioClip audio_Jump;
    public AudioClip audio_Land;
    public AudioClip audio_Running;
    public AudioClip audio_Roll;
    public AudioClip audio_Potion;
    public AudioClip audio_SwordHit1;
    public AudioClip audio_SwordHit2;
    public AudioClip audio_Death;

    // General Variables
    private bool playerDead = false;
    private bool isGrounded;
    private bool isLookingRight = true;
    private bool isRolling = false;
    private bool isAttack1;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private float lastHealTime = 0f;
    private float lastHitTime = 0f;
    private float lastTempHealthReduction = 0f;
    private bool isKnockedBack = false;
    private string groundLayer = "Floor";
    private float yVelocity;
    private float transitionDuration = 4f;
    private bool playedDeathAnimation = false;


    void Start()
    {
        velocity = new Vector2(speed, speed);
        characterBody = GetComponent<Rigidbody2D>();
        soundManager = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (animator.GetFloat("isRolling") == 1)
        {
            gameObject.layer = LayerMask.NameToLayer("rollingLayer");

        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

        //Determine if still
        if ((characterBody.velocity == Vector2.zero || animator.GetCurrentAnimatorStateInfo(0).IsName("player_Idle")) && !isKnockedBack)
        {
            characterBody.velocity = new Vector2(0, characterBody.velocity.y);
            animator.SetFloat("isMoving", 0);
        }

        //Stop Sound if Idle
        if (characterBody.velocity == Vector2.zero && animator.GetCurrentAnimatorStateInfo(0).IsName("player_Idle") && !playerDead)
        {
            soundManager.Stop();
        }

        //Determine if attacking
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_Attack2") || animator.GetCurrentAnimatorStateInfo(0).IsName("player_Attack"))
        {
            isAttacking = true;
        }

        else
        {
            isAttacking = false;
        }

        //Landing after being hit
        if (isGrounded && animator.GetFloat("isHit") == 1 && (Time.time - lastHitTime) >= 0.1f)
        {
            isKnockedBack = false;
            animator.SetFloat("isHit", 0);
            spriteRenderer.color = Color.white;
        }

        yVelocity = characterBody.velocity.y;
        if (yVelocity >= 1 || yVelocity <= -1 || yVelocity == 0)
        {
            animator.SetFloat("yVelocity", yVelocity);

        }

        //Change direction of player
        if (!isLookingRight)
        {
            animator.SetFloat("isLookingRight", 0);
            transform.Find("Character").GetComponent<SpriteRenderer>().flipX = true;
        }

        else if (isLookingRight)
        {
            animator.SetFloat("isLookingRight", 1);
            transform.Find("Character").GetComponent<SpriteRenderer>().flipX = false;
        }

        //Managing Temp HealthBar
        if (tempHealthBar.fillAmount >= healthBar.fillAmount)
        {
            float currentTime = Time.time;
            if ((currentTime - lastHitTime) >= 1f)
            {

                if ((currentTime - lastTempHealthReduction) >= 0.05f)
                {
                    tempHealthBar.fillAmount -= 0.01f;
                    lastTempHealthReduction = currentTime;
                }
            }

        }

        //Player Death
        if (healthBar.fillAmount <= 0 && isGrounded)
        {
            playerDead = true;
            animator.SetFloat("isDead", 1);
            if (!playedDeathAnimation)
            {
                soundManager.Stop();
                soundManager.PlayOneShot(audio_Death);
                StartCoroutine(PlayDeathAnimation());
            }
            playedDeathAnimation = true;
        }

        //Player Potions
        if (potionsLeft != 0)
        {
            potionsLeftText.text = "Potions Left: " + potionsLeft;
        }

        else
        {
            potionsLeftText.text = "No Potions Left";
        }

        //Player Movement
        if ((!isKnockedBack || animator.GetFloat("isRolling") == 1) && !playerDead && !animator.GetCurrentAnimatorStateInfo(0).IsName("player_Potion"))
        {

            // Jumping
            if (
            Input.GetKeyDown(KeyCode.Space)
            && isGrounded
            && animator.GetFloat("isRolling") == 0
            && staminaBar.fillAmount >= 0.2f
            )
            {
                soundManager.Stop();
                soundManager.PlayOneShot(audio_Jump);
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
            && potionsLeft != 0
            && healthBar.fillAmount < 1
            )
            {
                soundManager.Stop();
                potionsLeft -= 1;
                soundManager.PlayOneShot(audio_Potion);
                float currentHealTime = Time.time;
                if ((currentHealTime - lastHealTime) >= 1.1f)
                {
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
            && staminaBar.fillAmount >= 0.3f)
            {
                soundManager.Stop();
                isRolling = true;
                soundManager.PlayOneShot(audio_Roll);
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
            if (//hello
            Input.GetKeyDown(KeyCode.K)
            && !(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            && animator.GetFloat("isRolling") == 0
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("player_Attack2")
            && staminaBar.fillAmount >= 0.15f
            && !isAttacking
            && isGrounded)
            {
                soundManager.Stop();
                //Timing and Animation
                float currentAttackTime = Time.time;
                float difference = currentAttackTime - lastAttackTime;
                if (difference >= 0.6f)
                {
                    isAttack1 = true;
                }
                if (isAttack1 && !animator.GetCurrentAnimatorStateInfo(0).IsName("player_Attack"))
                {
                    soundManager.PlayOneShot(audio_SwordHit2);
                    StartCoroutine(AllowForAnimation("isAttacking1", 0.3f));
                }
                else
                {
                    soundManager.PlayOneShot(audio_SwordHit2);
                    StartCoroutine(AllowForAnimation("isAttacking2", 0.3f));
                }
                staminaBar.fillAmount -= 0.15f;
                isAttack1 = !isAttack1;
                lastAttackTime = currentAttackTime;

                //Damage
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
                foreach (Collider2D enemy in hitEnemies)
                {
                    if (enemy.CompareTag("Enemy")) // Ensure your enemy GameObjects have the "Enemy" tag
                    {
                        if (enemy.GetComponent<Rigidbody2D>().transform.position.x - characterBody.position.x >= 0
                        && enemy.GetComponent<Rigidbody2D>().transform.position.y - characterBody.position.y < 2
                        && isLookingRight)
                        {
                            enemy.GetComponent<Enemy>().TakeDamage(30, transform.position);
                        }

                        else if (enemy.GetComponent<Rigidbody2D>().transform.position.x - characterBody.position.x < 0
                                                    && enemy.GetComponent<Rigidbody2D>().transform.position.y - characterBody.position.y < 2

                        && !isLookingRight)
                        {
                            enemy.GetComponent<Enemy>().TakeDamage(30, transform.position);
                        }
                    }
                }

            }


            //Moving Left
            if (Input.GetKey(KeyCode.A) && animator.GetFloat("isRolling") == 0)
            {
                if (isGrounded && soundManager.isPlaying == false)
                {
                    soundManager.PlayOneShot(audio_Running);
                }
                isLookingRight = false;
                animator.SetFloat("isMoving", 1);
                characterBody.velocity = new Vector2(-speed, characterBody.velocity.y); // Move left
            }

            // Moving Right
            if (Input.GetKey(KeyCode.D) && animator.GetFloat("isRolling") == 0)
            {
                if (isGrounded && soundManager.isPlaying == false)
                {
                    soundManager.PlayOneShot(audio_Running);
                }
                isLookingRight = true;
                animator.SetFloat("isMoving", 1);
                characterBody.velocity = new Vector2(speed, characterBody.velocity.y); // Move right
            }

            if ((Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A)))
            {
                animator.SetFloat("isMoving", 0);
                if (animator.GetFloat("isRolling") == 0 && !isKnockedBack && isGrounded)
                {
                    characterBody.velocity = new Vector2(0, characterBody.velocity.y);
                }
            }
        }



    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Shows the attack radius
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

    IEnumerator AllowForAnimationDeath()
    {
        yield return new WaitForSeconds(0.3f);
        // Destroy(gameObject);
    }


    public void DamagePlayer(float amount, Vector2 EnemyPosition, float knockbackAmount = 5f)
    {
        if (animator.GetFloat("isRolling") != 1 && healthBar.fillAmount > 0 && !invincible)
        {
            soundManager.Stop();
            soundManager.PlayOneShot(audio_Hurt);
            if (healthBar.fillAmount > 0)
            {
                healthBar.fillAmount -= amount;
                characterBody.velocity = new Vector2(0, 0);
                isKnockedBack = true;
                //If enemy is to the right of player
                if (EnemyPosition.x >= transform.position.x)
                {
                    if (!isLookingRight)
                    {
                        isLookingRight = true;
                    }
                    characterBody.AddForce(Vector2.up * knockbackAmount, ForceMode2D.Impulse);
                    characterBody.AddForce(Vector2.left * knockbackAmount, ForceMode2D.Impulse);
                }

                //If enemy is to the left of player
                else
                {
                    if (isLookingRight)
                    {
                        isLookingRight = false;
                    }
                    characterBody.AddForce(Vector2.up * knockbackAmount, ForceMode2D.Impulse);
                    characterBody.AddForce(Vector2.right * knockbackAmount, ForceMode2D.Impulse);
                }
                lastHitTime = Time.time;
                animator.SetFloat("isHit", 1);
            }

        }

    }

    void PlaySound(AudioClip clip)
    {
        soundManager.PlayOneShot(clip);
    }

    private IEnumerator PlayDeathAnimation()
    {
        float elapsedTime = 0f;
        Color color = Deathscreen.color;
        color.a = 0f; // Start with transparent
        Deathscreen.color = color;

        while (elapsedTime < transitionDuration && Deathscreen.color.a != 1f)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / transitionDuration);
            Deathscreen.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        TransitionImage.gameObject.SetActive(true);
        Deathscreen.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        elapsedTime = 0f;
        color = TransitionImage.color;
        color.a = 0f; // Start with transparent
        TransitionImage.color = color;
        Color deathscreenColor = Deathscreen.color;
        float originalYoudiedA = Deathscreen.color.a;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / transitionDuration);
            TransitionImage.color = new Color(color.r, color.g, color.b, alpha);
            Deathscreen.color = new Color(deathscreenColor.r, deathscreenColor.g, deathscreenColor.b, originalYoudiedA - alpha);
            yield return null;
        }

        SceneManager.LoadScene("MainMenu");
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class Boss : Enemy
{
    Animator animator;
    private float lastHit = 0f;
    private float lastPosition = 0f;
    private float lastPositionTime = 0f;
    private float lastHitTime = 0f;
    private float lastTempHealthReduction = 0f;
    private bool isIdle = false;
    private bool invincible = false;
    private float lastDamageTime = 0f;
    private float transitionDuration = 2f;
    public Image transitionImage;

    public Image healthBar;
    public Image tempHealthBar;
    public GameObject player;
    public UnityEngine.UI.Image Deathscreen;
    public AudioSource backgroundMusic;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 300;
        base.Start();
        isBoss = true;
        animator = GetComponent<Animator>();

        if (GetComponent<AudioSource>() == null)
        {
            gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        float currentTime = Time.time;
        if (currentTime - lastDamageTime < 1.25f)
        {
            invincible = true;
        }
        else
        {
            invincible = false;
        }

        if (activated)
        {
            AttackPlayer();
        }

        //Animations
        //Idle
        currentTime = Time.time;
        if (currentTime - lastPositionTime > 0.1f)
        {
            if (lastPosition == transform.position.x)
            {
                animator.SetFloat("isMoving", 0);
                isIdle = true;
            }
            else
            {
                isIdle = false;
            }
            lastPosition = transform.position.x;
            lastPositionTime = currentTime;
        }


        //Running
        if (!isIdle && canMove && isGrounded && animator.GetFloat("isAttack1") == 0 && animator.GetFloat("isAttack2") == 0 && animator.GetFloat("isAttack3") == 0 && animator.GetFloat("isHurt") == 0 && animator.GetFloat("isDead") == 0)
        {

            animator.SetFloat("isMoving", 1);

        }
        else
        {
            animator.SetFloat("isMoving", 0);
        }

        healthBar.fillAmount = (float)health / (float)maxHealth;
        if (tempHealthBar.fillAmount >= healthBar.fillAmount)
        {
            currentTime = Time.time;
            if ((currentTime - lastHitTime) >= 1f)
            {

                if ((currentTime - lastTempHealthReduction) >= 0.05f)
                {
                    tempHealthBar.fillAmount -= 0.01f;
                    lastTempHealthReduction = currentTime;
                }
            }

        }

    }

    public override void AttackPlayer()
    {
        if (health <= 0)
        {
            return;
        }

        float currentHit = Time.time;
        float playerPositionX = player.transform.position.x;
        float playerPositionY = player.transform.position.y;
        float enemyPositionX = transform.position.x;
        float enemyPositionY = transform.position.y;

        if (currentHit - lastHit > 3f && canMove)
        {

            int randInt = UnityEngine.Random.Range(1, 4);
            if (randInt == 1)
            {
                StartCoroutine(Attack1());
            }
            else if (randInt == 2)
            {
                StartCoroutine(Attack2());
            }
            else if (randInt == 3)
            {
                StartCoroutine(Attack3());
            }

            lastHit = Time.time;

            UnityEngine.Debug.Log("Attacking Player");
            UnityEngine.Debug.Log("Player Position: " + playerPositionX + ", " + playerPositionY);
            UnityEngine.Debug.Log("Enemy Position: " + enemyPositionX + ", " + enemyPositionY);
            UnityEngine.Debug.Log("Distance: " + Mathf.Abs(playerPositionX - enemyPositionX) + ", " + Mathf.Abs(playerPositionY - enemyPositionY));


        }
    }


    public override void TakeDamage(int damage, Vector3 playerPosition)
    {
        if (!invincible)
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(Resources.Load<AudioClip>("Enemies/Orcs/OrcHit"));
            health -= damage;
            if (health <= 0)
            {
                StartCoroutine(Death());
            }
            else
            {
                float playerPositionX = playerPosition.x;
                float enemyPositionX = transform.position.x;

                characterBody.velocity = new Vector2(0, 0);

                characterBody.AddForce(Vector2.up * knockbackAmount, ForceMode2D.Impulse);
                if (playerPositionX >= enemyPositionX)
                {
                    spriteRenderer.color = Color.red;
                    characterBody.AddForce(Vector2.left * knockbackAmount * 2, ForceMode2D.Impulse);
                }
                else
                {
                    spriteRenderer.color = Color.red;
                    characterBody.AddForce(Vector2.right * knockbackAmount * 2, ForceMode2D.Impulse);
                }

                StartCoroutine(Hurt());
            }
            lastDamageTime = Time.time;
        }
    }

    private IEnumerator Hurt()
    {
        canMove = false;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            UnityEngine.Debug.LogError("AudioSource component is missing on the Orc GameObject.");
        }
        else
        {
            AudioClip hitClip = Resources.Load<AudioClip>("Enemies/Boss/Hurt");
            if (hitClip == null)
            {
                UnityEngine.Debug.LogError("Failed to load audio clip");
            }
            else
            {
                audioSource.PlayOneShot(hitClip);
            }
        }

        animator.SetFloat("isHurt", 1);
        yield return new WaitForSeconds(0.5f);
        animator.SetFloat("isHurt", 0);
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    private IEnumerator Death()
    {
        canMove = false;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            UnityEngine.Debug.LogError("AudioSource component is missing on the Orc GameObject.");
        }
        else
        {
            AudioClip hitClip = Resources.Load<AudioClip>("game-start");
            AudioClip WonGame = Resources.Load<AudioClip>("Enemies/Boss/Attack1");
            if (hitClip == null)
            {
                UnityEngine.Debug.LogError("Failed to load audio clip");
            }
            else
            {
                audioSource.PlayOneShot(hitClip);
                audioSource.PlayOneShot(WonGame);
            }
        }

        animator.SetFloat("isDead", 1);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(ReduceAlphaOverTime(5f));
        yield return new WaitForSeconds(5f);
        StartCoroutine(FadeAndLoadScene());
    }


    private IEnumerator ReduceAlphaOverTime(float duration)
    {
        characterBody.velocity = new Vector2(0, 0);
        transform.position = new Vector2(transform.position.x, transform.position.y);
        characterBody.bodyType = RigidbodyType2D.Kinematic;
        Color initialColor = spriteRenderer.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(initialColor.a, 0, elapsedTime / duration);
            spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
        //Die();
    }

    private IEnumerator Attack1()
    {
        canMove = false;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            UnityEngine.Debug.LogError("AudioSource component is missing on the Orc GameObject.");
        }
        else
        {
            AudioClip hitClip = Resources.Load<AudioClip>("Enemies/Boss/Attack1");
            if (hitClip == null)
            {
                UnityEngine.Debug.LogError("Failed to load audio clip");
            }
            else
            {
                audioSource.PlayOneShot(hitClip);
            }
        }

        characterBody.velocity = new Vector2(0, characterBody.velocity.y);

        float playerPositionX = player.transform.position.x;
        float playerPositionY = player.transform.position.y;
        float enemyPositionX = transform.position.x;
        float enemyPositionY = transform.position.y;

        if (Mathf.Abs(playerPositionX - enemyPositionX) < 3 && Mathf.Abs(playerPositionY - enemyPositionY) < 5)
        {
            playerMovement.DamagePlayer(0.3f, transform.position);
        }

        animator.SetFloat("isAttack1", 1);
        yield return new WaitForSeconds(0.4f);
        animator.SetFloat("isAttack1", 0);
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    private IEnumerator Attack2()
    {
        canMove = false;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            UnityEngine.Debug.LogError("AudioSource component is missing on the Orc GameObject.");
        }
        else
        {
            AudioClip hitClip = Resources.Load<AudioClip>("Enemies/Boss/Attack2");
            if (hitClip == null)
            {
                UnityEngine.Debug.LogError("Failed to load audio clip");
            }
            else
            {
                audioSource.PlayOneShot(hitClip);
            }
        }

        characterBody.velocity = new Vector2(0, characterBody.velocity.y);
        float playerPositionX = player.transform.position.x;
        float playerPositionY = player.transform.position.y;
        float enemyPositionX = transform.position.x;
        float enemyPositionY = transform.position.y;

        if (Mathf.Abs(playerPositionX - enemyPositionX) < 3 && Mathf.Abs(playerPositionY - enemyPositionY) < 5)
        {
            playerMovement.DamagePlayer(0.3f, transform.position);
        }
        animator.SetFloat("isAttack2", 1);
        yield return new WaitForSeconds(0.5f);
        animator.SetFloat("isAttack2", 0);
        yield return new WaitForSeconds(1f);
        canMove = true;
    }


    private IEnumerator Attack3()
    {
        canMove = false;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            UnityEngine.Debug.LogError("AudioSource component is missing on the Orc GameObject.");
        }
        else
        {
            AudioClip hitClip = Resources.Load<AudioClip>("Enemies/Boss/Attack3");
            if (hitClip == null)
            {
                UnityEngine.Debug.LogError("Failed to load audio clip");
            }
            else
            {
                audioSource.PlayOneShot(hitClip);
            }
        }

        characterBody.velocity = new Vector2(0, characterBody.velocity.y);
        float playerPositionX = player.transform.position.x;
        float playerPositionY = player.transform.position.y;
        float enemyPositionX = transform.position.x;
        float enemyPositionY = transform.position.y;

        if (Mathf.Abs(playerPositionX - enemyPositionX) < 3 && Mathf.Abs(playerPositionY - enemyPositionY) < 5)
        {
            playerMovement.DamagePlayer(0.3f, transform.position);
        }
        animator.SetFloat("isAttack3", 1);
        yield return new WaitForSeconds(0.4f);
        animator.SetFloat("isAttack3", 0);
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

        private IEnumerator FadeAndLoadScene()
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

        transitionImage.gameObject.SetActive(true);
        Deathscreen.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        elapsedTime = 0f;
        color = transitionImage.color;
        color.a = 0f; // Start with transparent
        transitionImage.color = color;
        Color deathscreenColor = Deathscreen.color;
        float originalYoudiedA = Deathscreen.color.a;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / transitionDuration);
            transitionImage.color = new Color(color.r, color.g, color.b, alpha);
            Deathscreen.color = new Color(deathscreenColor.r, deathscreenColor.g, deathscreenColor.b, originalYoudiedA - alpha);
            yield return null;
        }

        SceneManager.LoadScene("MainMenu");
    }
}


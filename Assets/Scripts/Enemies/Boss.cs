using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Enemy
{
    Animator animator;
    private float lastHit = 0f;
    private float lastPosition = 0f;
    private float lastPositionTime = 0f;
    private float lastHitTime = 0f;
    private float lastTempHealthReduction = 0f;
    private bool isIdle = false;
    public Image healthBar;
    public Image tempHealthBar;    

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
        if (activated)
        {
            AttackPlayer();
        }

        //Animations
        //Idle
        float currentTime = Time.time;
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
        if (currentHit - lastHit > 3f)
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

        }
    }


    public override void TakeDamage(int damage, Vector3 playerPosition)
    {
        Debug.Log("Boss taking damage");
        //base.TakeDamage(damage, playerPosition);
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
    }

    private IEnumerator Hurt()
    {
        canMove = false;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the Orc GameObject.");
        }
        else
        {
            AudioClip hitClip = Resources.Load<AudioClip>("Enemies/Orcs/OrcHit");
            if (hitClip == null)
            {
                Debug.LogError("Failed to load audio clip");
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
            Debug.LogError("AudioSource component is missing on the Orc GameObject.");
        }
        else
        {
            AudioClip hitClip = Resources.Load<AudioClip>("Enemies/Orcs/OrcDeath");
            if (hitClip == null)
            {
                Debug.LogError("Failed to load audio clip");
            }
            else
            {
                audioSource.PlayOneShot(hitClip);
            }
        }

        animator.SetFloat("isDead", 1);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(ReduceAlphaOverTime(5f));
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
        Die();
    }

    private IEnumerator Attack1()
    {
        canMove = false;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the Orc GameObject.");
        }
        else
        {
            AudioClip hitClip = Resources.Load<AudioClip>("Enemies/Orcs/OrcAttack");
            if (hitClip == null)
            {
                Debug.LogError("Failed to load audio clip");
            }
            else
            {
                audioSource.PlayOneShot(hitClip);
            }
        }

        characterBody.velocity = new Vector2(0, characterBody.velocity.y);
        playerMovement.DamagePlayer(0.3f, transform.position);
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
            Debug.LogError("AudioSource component is missing on the Orc GameObject.");
        }
        else
        {
            AudioClip hitClip = Resources.Load<AudioClip>("Enemies/Orcs/OrcAttack");
            if (hitClip == null)
            {
                Debug.LogError("Failed to load audio clip");
            }
            else
            {
                audioSource.PlayOneShot(hitClip);
            }
        }

        characterBody.velocity = new Vector2(0, characterBody.velocity.y);
        playerMovement.DamagePlayer(0.3f, transform.position);
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
            Debug.LogError("AudioSource component is missing on the Orc GameObject.");
        }
        else
        {
            AudioClip hitClip = Resources.Load<AudioClip>("Enemies/Orcs/OrcAttack");
            if (hitClip == null)
            {
                Debug.LogError("Failed to load audio clip");
            }
            else
            {
                audioSource.PlayOneShot(hitClip);
            }
        }

        characterBody.velocity = new Vector2(0, characterBody.velocity.y);
        playerMovement.DamagePlayer(0.3f, transform.position);
        animator.SetFloat("isAttack3", 1);
        yield return new WaitForSeconds(0.4f);
        animator.SetFloat("isAttack3", 0);
        yield return new WaitForSeconds(1f);
        canMove = true;
    }
}


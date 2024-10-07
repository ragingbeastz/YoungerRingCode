using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Boss : Enemy
{
    Animator animator;
    private float lastHit = 0f;
    private float lastPosition = 0f;
    private float lastPositionTime = 0f;
    private bool isIdle = false;
    // Start is called before the first frame update
    void Start()
    {
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
                Debug.Log("Idle");
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
        base.TakeDamage(damage, playerPosition);
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Enemies/Orcs/OrcHit"));
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


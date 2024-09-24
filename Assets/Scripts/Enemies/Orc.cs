using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : Enemy
{
    Animator animator;
    private float lastHit = 0f;
    private float lastPosition = 0f;
    private float lastPositionTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();

        // Ensure AudioSource component is attached
        if (GetComponent<AudioSource>() == null)
        {
            gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        AttackPlayer();

        float currentPosition = transform.position.x;
        float currentTime = Time.time;

        if (currentPosition != lastPosition)
        {
            lastPosition = currentPosition;
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
        if (currentHit - lastHit > 1f)
        {
            if (Math.Abs(playerMovement.transform.position.x - transform.position.x) < 2)
            {
                StartCoroutine(HitPlayer());
                lastHit = Time.time;
            }
        }

    }

    public override void TakeDamage(int damage, Vector3 playerPosition)
    {
        base.TakeDamage(damage, playerPosition);
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Enemies/Orcs/OrcHit"));

    }

    protected override void Die(){
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Enemies/Orcs/OrcDeath"));
        base.Die();
    }

    private IEnumerator HitPlayer()
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

        animator.SetFloat("isHitting", 1);
        characterBody.velocity = new Vector2(0, characterBody.velocity.y);
        playerMovement.DamagePlayer(0.3f, transform.position);
        yield return new WaitForSeconds(0.3f);
        animator.SetFloat("isHitting", 0);
        yield return new WaitForSeconds(1f);
        canMove = true;
    }
}

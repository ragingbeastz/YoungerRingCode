using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Enemy
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
        healthBarPosition = new Vector2(0.28f, -6.16f);

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
        if (currentHit - lastHit > 3f)
        {

            if (Math.Abs(playerMovement.transform.position.x - transform.position.x) < 3)
            {
                StartCoroutine(HitPlayerFlame());
            }

            else
            {
                StartCoroutine(ThrowFireballCoroutine());
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

    protected override void Die()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Enemies/Orcs/OrcDeath"));
        base.Die();
    }

    private IEnumerator HitPlayerFlame()
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

        animator.SetFloat("isFlame", 1);
        yield return new WaitForSeconds(0.3f);
        animator.SetFloat("isFlame", 0);
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    private IEnumerator ThrowFireballCoroutine()
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

        animator.SetFloat("isFireball", 1);
        yield return new WaitForSeconds(0.3f);
        animator.SetFloat("isFireball", 0);
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    private void FlamePlayer()
    {

        float playerDistanceFromEnemy = playerMovement.transform.position.x - transform.position.x;
        if (isFacingRight && playerDistanceFromEnemy >= 0 && Math.Abs(playerDistanceFromEnemy) < 5)
        {
            playerMovement.DamagePlayer(0.3f, transform.position);
        }
        else if (!isFacingRight && playerDistanceFromEnemy <= 0 && Math.Abs(playerDistanceFromEnemy) < 5)
        {
            playerMovement.DamagePlayer(0.3f, transform.position);
        }

    }

    private void ThrowFireball()
    {

        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Enemies/Skeletons/BowFire"));

        GameObject fireball = new GameObject("fireball");
        //fireball.transform.SetParent(transform);
        fireball.AddComponent<SpriteRenderer>();
        fireball.AddComponent<Rigidbody2D>();
        fireball.AddComponent<BoxCollider2D>();

        //Animation
        SpriteRenderer fireballSprite = fireball.GetComponent<SpriteRenderer>();
        fireball.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Enemies/Wizards/Fireball");
        fireball.transform.localScale = new Vector3(1f, 1f, 1f);
        

        //Positioning
        if (isFacingRight)
        {
            fireball.transform.position = new Vector2(transform.position.x + 1.56f, transform.position.y - 0.35f);
            fireball.transform.rotation = Quaternion.Euler(0, 0, 9.6f);
        }
        else
        {
            fireball.transform.position = new Vector2(transform.position.x - 1.56f, transform.position.y - 0.35f);
            fireball.transform.rotation = Quaternion.Euler(0, 0, -9.6f);
            fireballSprite.flipX = true;
        }


        //Physics
        Rigidbody2D fireballBody = fireball.GetComponent<Rigidbody2D>();
        fireballBody.gravityScale = 0;
        fireballBody.angularVelocity = -2000f;
        fireballBody.angularDrag = 0f; // Set the angular velocity to make the fireball spin
        fireballBody.mass = 0f;
        if (isFacingRight)
        {
            fireballBody.velocity = new Vector2(3, 0);
        }
        else
        {
            fireballBody.velocity = new Vector2(-3, 0);
        }

        //Collisions
        fireball.layer = LayerMask.NameToLayer("Enemy");
        int rollingLayer = LayerMask.NameToLayer("rollingLayer");
        Physics2D.IgnoreLayerCollision(fireball.layer, rollingLayer);
        BoxCollider2D fireballCollider = fireball.GetComponent<BoxCollider2D>();
        fireballCollider.size = new Vector2(0.4803897f, 0.05234949f);
        fireballCollider.offset = new Vector2(0.004857153f, -0.005216029f);
        fireball.AddComponent<fireballCollision>();

        StartCoroutine(WaitandDestroy(fireball));

    }

    private IEnumerator WaitandDestroy(GameObject fireball)
    {
        yield return new WaitForSeconds(5f);
        Destroy(fireball);
    }


}

    class fireballCollision : MonoBehaviour{
        

        void OnCollisionEnter2D(Collision2D collision){
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
            collision.gameObject.GetComponent<PlayerMovement>().DamagePlayer(0.1f, transform.position);
            Destroy(gameObject);
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
            {
            Rigidbody2D fireballBody = GetComponent<Rigidbody2D>();
            fireballBody.velocity = new Vector2(0, 0);
            fireballBody.bodyType = RigidbodyType2D.Kinematic;
            fireballBody.angularVelocity = 0;
            fireballBody.gravityScale = 0;

            BoxCollider2D fireballCollider = GetComponent<BoxCollider2D>();
            fireballCollider.enabled = false; // Disable collision
            Destroy(gameObject);
            }
        }


    }

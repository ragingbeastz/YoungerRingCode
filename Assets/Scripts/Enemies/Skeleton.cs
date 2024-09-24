using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Skeleton : Enemy
{
    Animator animator;
    private float lastHit = 0f;
    private float lastPosition = 0f;
    private float lastPositionTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        canMove = false;
        healthBarPosition = new Vector2(-0.21f, -7.5f);

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
    }

    public override void AttackPlayer()
    {
        if (health <= 0)
        {
            return;
        }
        float currentHit = Time.time;

        if (currentHit - lastHit > 5f)
        {
            StartCoroutine(HitPlayer());
            lastHit = Time.time;
        }

    }

    public override void TakeDamage(int damage, Vector3 playerPosition)
    {
        base.TakeDamage(damage, playerPosition);
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Enemies/Skeletons/SkeletonHit"));

    }

    protected override void Die(){
        audioSource = GetComponent<AudioSource>();
        //audioSource.PlayOneShot(Resources.Load<AudioClip>("Enemies/Orcs/OrcDeath"));
        base.Die();
    }

    private IEnumerator HitPlayer()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the Orc GameObject.");
        }
        else
        {
            AudioClip hitClip = Resources.Load<AudioClip>("Enemies/Skeletons/BowDraw");
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
        yield return new WaitForSeconds(0.3f);
        animator.SetFloat("isHitting", 0);
        yield return new WaitForSeconds(1f);
    }

    void ShootArrow(){
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Enemies/Skeletons/BowFire"));

        GameObject arrow = new GameObject("Arrow");
        arrow.transform.SetParent(transform);
        arrow.AddComponent<SpriteRenderer>();
        arrow.AddComponent<Rigidbody2D>();
        arrow.AddComponent<BoxCollider2D>();        

        //Sprite
        SpriteRenderer arrowSprite = arrow.GetComponent<SpriteRenderer>();
        arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Enemies/Skeletons/Arrow");

        //Positioning
        if(isFacingRight){
        arrow.transform.position = new Vector2(transform.position.x + 1.56f, transform.position.y - 0.35f);
        arrow.transform.rotation = Quaternion.Euler(0, 0, 9.6f);
        }
        else{
            arrow.transform.position = new Vector2(transform.position.x - 1.56f, transform.position.y - 0.35f);
            arrow.transform.rotation = Quaternion.Euler(0, 0, -9.6f);
            arrowSprite.flipX = true;        
            }
        
        arrow.transform.localScale = new Vector3(1, 1, 1);

        //Physics
        Rigidbody2D arrowBody = arrow.GetComponent<Rigidbody2D>();
        arrowBody.gravityScale = 1;
        if (isFacingRight){
            arrowBody.velocity = new Vector2(10, 2.5f);
            arrowBody.angularVelocity = -25f;
        }
        else{
            arrowBody.velocity = new Vector2(-10, 2.5f);
            arrowBody.angularVelocity = 25f;
        }

        //Collisions
        arrow.layer = LayerMask.NameToLayer("Enemy");
        int rollingLayer = LayerMask.NameToLayer("rollingLayer");
        Physics2D.IgnoreLayerCollision(arrow.layer, rollingLayer);
        BoxCollider2D arrowCollider = arrow.GetComponent<BoxCollider2D>();
        arrowCollider.size = new Vector2(0.4803897f, 0.05234949f);
        arrowCollider.offset = new Vector2(0.004857153f, -0.005216029f);
        arrow.AddComponent<ArrowCollision>();

        

    }

    class ArrowCollision : MonoBehaviour{

        void OnCollisionEnter2D(Collision2D collision){
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
            collision.gameObject.GetComponent<PlayerMovement>().DamagePlayer(0.1f, transform.position);
            Destroy(gameObject);
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
            {
            Rigidbody2D arrowBody = GetComponent<Rigidbody2D>();
            arrowBody.velocity = new Vector2(0, 0);
            arrowBody.bodyType = RigidbodyType2D.Kinematic;
            arrowBody.angularVelocity = 0;
            arrowBody.gravityScale = 0;

            BoxCollider2D arrowCollider = GetComponent<BoxCollider2D>();
            arrowCollider.enabled = false; // Disable collision
            StartCoroutine(ArrowDestroy());
            }
        }

        IEnumerator ArrowDestroy(){
            yield return new WaitForSeconds(3f);
            Destroy(gameObject);
        }
    }
}


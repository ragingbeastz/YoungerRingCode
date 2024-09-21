using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected int health = 100;
    protected bool isGrounded;
    private SpriteRenderer spriteRenderer;
    private string groundLayer = "Floor"; 
    private Color originalColor = Color.white;



    public virtual void TakeDamage(int damage, Vector3 playerPosition)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        health -= damage;

        if (health <= 0)
        {
            Die();
        }

        spriteRenderer.color = Color.red;
        float playerPositionX = playerPosition.x;
        float enemyPositionX = transform.position.x;

        Rigidbody2D characterBody = GetComponent<Rigidbody2D>();
        characterBody.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        if (playerPositionX >= enemyPositionX)
        {
            spriteRenderer.color = Color.red;
            characterBody.AddForce(Vector2.left * 10, ForceMode2D.Impulse);
        }
        else
        {
            spriteRenderer.color = Color.red;
            characterBody.AddForce(Vector2.right * 10, ForceMode2D.Impulse);
        }
        
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayer))
        {
            isGrounded = true;
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.white;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayer))
        {
            isGrounded = false;
        }
    }
}

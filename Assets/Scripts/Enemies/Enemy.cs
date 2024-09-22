using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    protected int health = 100;
    protected bool isGrounded;
    private SpriteRenderer spriteRenderer;
    private string groundLayer = "Floor"; 
    private Color originalColor = Color.white;

    //Health Bar
    GameObject canvasGameObject;
    GameObject healthBarGameObject;
    GameObject healthBarRectangleGameObject;
    Canvas canvas;
    Image healthBarImage;


    protected virtual void Start(){

        canvasGameObject = new GameObject("EnemyCanvas");
        canvas = canvasGameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasGameObject.transform.SetParent(transform);

        canvasGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 50); // Example size for the Canvas
        canvasGameObject.transform.localPosition = new Vector3(0, 2, 0); // Position it above the enemy

        healthBarGameObject = new GameObject("HealthBar");
        healthBarGameObject.transform.SetParent(canvasGameObject.transform);

        healthBarImage = healthBarGameObject.AddComponent<UnityEngine.UI.Image>();
        healthBarImage.color = Color.red;

        // Configure the RectTransform of the HealthBar
        RectTransform healthBarRect = healthBarGameObject.GetComponent<RectTransform>();
        healthBarRect.sizeDelta = new Vector2(1.5f, 0.1f);  // Width 100, Height 10
        healthBarRect.anchoredPosition = new Vector2(0,-1f);  // Set its position to the center of the canvas

        Debug.Log(healthBarImage.color);

    }

    protected virtual void Update(){

    }



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

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public abstract class Enemy : MonoBehaviour
{
    // Variables
    private bool hasTakenDamage = false;
    private Color originalColor = Color.white;
    private bool scaleReachedZero = false;
    private string groundLayer = "Floor";
    private float lastMovement = 0f;

    // Health
    protected float health;
    protected float maxHealth = 100;

    // Player
    public GameObject Player;
    protected PlayerMovement playerMovement;

    // Movement
    protected int direction;
    protected float knockbackAmount = 5;
    protected Vector2 velocity;
    protected int speed = 5;
    protected bool isGrounded;
    protected bool canMove = true;
    protected bool isHit = false;

    // Components
    protected Rigidbody2D characterBody;
    protected SpriteRenderer spriteRenderer;
    protected AudioSource audioSource;

    // State
    protected bool isFacingRight = true;

    //Health Bar
    GameObject canvasGameObject;
    GameObject healthBarGameObject;
    GameObject bgImageGameObject;
    GameObject healthBarRectangleGameObject;
    GameObject sliderGameObject;
    Canvas canvas;
    UnityEngine.UI.Image healthBarImage;
    UnityEngine.UI.Image bgImage;
    UnityEngine.UI.Slider slider;
    GameObject sliderObject;




    protected virtual void Start()
    {
        health = maxHealth;
        velocity = new Vector2(speed, speed);
        characterBody = GetComponent<Rigidbody2D>();
        playerMovement = Player.GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();



        canvasGameObject = new GameObject("EnemyCanvas");
        canvas = canvasGameObject.AddComponent<Canvas>();
        canvasGameObject.transform.SetParent(transform);
        canvasGameObject.transform.localPosition = new Vector3(0, 2, 0);
    }

    protected virtual void Update()
    {
        FacePlayer();
        if (canMove && health > 0 && isGrounded)
        {
            enemyMove();
        }
    }

    public abstract void AttackPlayer();


    public virtual void TakeDamage(int damage, Vector3 playerPosition)
    {
        isHit = true;
        if (!hasTakenDamage)
        {
            hasTakenDamage = true;

            healthBarGameObject = new GameObject("HealthBar");
            healthBarGameObject.transform.SetParent(canvasGameObject.transform);

            bgImageGameObject = new GameObject("BackgroundImage");
            bgImageGameObject.transform.SetParent(canvasGameObject.transform);

            Texture2D texture = LoadTexture("C:\\Users\\Dimithri\\Desktop\\Coding\\YoungerRingCode\\Library\\PackageCache\\com.unity.2d.sprite@1.0.0\\Editor\\ObjectMenuCreation\\DefaultAssets\\Textures\\v2\\Square.png");
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            //Background Image
            bgImage = bgImageGameObject.AddComponent<UnityEngine.UI.Image>();

            RectTransform bgImageRect = bgImageGameObject.GetComponent<RectTransform>();
            bgImageRect.sizeDelta = new Vector2(1.5f, 0.1f);
            bgImageRect.anchoredPosition = new Vector2(0, -5.5f);
            bgImageRect.anchorMin = new Vector2(0.5f, 0.5f);
            bgImageRect.anchorMax = new Vector2(0.5f, 0.5f);
            bgImageRect.pivot = new Vector2(0.5f, 0.5f);

            bgImage.sprite = sprite;
            bgImage.color = Color.black;

            //Health Bar Image
            healthBarImage = healthBarGameObject.AddComponent<UnityEngine.UI.Image>();

            RectTransform healthBarRect = healthBarGameObject.GetComponent<RectTransform>();
            healthBarRect.sizeDelta = new Vector2(1.5f, 0.1f);
            healthBarRect.anchoredPosition = new Vector2(0, -5.5f);
            healthBarRect.anchorMin = new Vector2(0.5f, 0.5f);
            healthBarRect.anchorMax = new Vector2(0.5f, 0.5f);
            healthBarRect.pivot = new Vector2(0.5f, 0.5f);

            healthBarImage.sprite = sprite;
            healthBarImage.color = Color.red;
            healthBarImage.type = UnityEngine.UI.Image.Type.Filled;
            healthBarImage.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            healthBarImage.fillOrigin = 0;
            healthBarImage.fillAmount = 1f;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        health -= damage;


        float healthPercentage = health / maxHealth;

        healthBarImage.fillAmount = healthPercentage;

        if (health <= 0)
        {
            if (scaleReachedZero)
            {
                Die();
            }
            else
            {
                StartCoroutine(ReduceAlphaOverTime(2f)); // Reduce scale over 1 second
            }
        }

        spriteRenderer.color = Color.red;
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

            if (isHit)
            {
                isHit = false;
                characterBody.velocity = new Vector2(0, 0);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayer))
        {
            isGrounded = false;
        }
    }

    Texture2D LoadTexture(string path)
    {
        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            return texture;
        }
        else
        {
            Debug.LogError("File not found: " + path);
            return null;
        }
    }

    void FacePlayer()
    {
        //Make Direction of travel towards player
        if (Player != null)
        {
            Vector2 PlayerPosition = Player.transform.position;
            Vector2 EnemyyPosition = transform.position;

            float PlayerX = PlayerPosition.x;
            float EnemyX = EnemyyPosition.x;

            //Rightward Direction
            if (PlayerX >= EnemyX)
            {
                direction = 1;
                if (!isFacingRight)
                {
                    isFacingRight = true;
                    spriteRenderer.flipX = false;
                }
            }
            //Leftward Direction
            else
            {
                direction = -1;
                if (isFacingRight)
                {
                    isFacingRight = false;
                    spriteRenderer.flipX = true;
                }
            }
        }
    }

    void enemyMove()
    {
        float lastPosition = transform.position.x;
        float newPosition = transform.position.x;
        if ((transform.position.x - Player.transform.position.x) >= 0.5 || (transform.position.x - Player.transform.position.x) <= -0.5
        && Mathf.Abs(transform.position.x - Player.transform.position.x) <= 10)
        {
            if (isFacingRight)
            {
                transform.Translate(Vector2.right * speed * Time.deltaTime);

            }
            else
            {
                transform.Translate(Vector2.left * speed * Time.deltaTime);
            }
            newPosition = transform.position.x;
        
        }
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


}

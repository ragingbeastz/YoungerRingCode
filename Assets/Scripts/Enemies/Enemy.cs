using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Enemy : MonoBehaviour
{
    public GameObject Player;
    private int direction;
    protected float knockbackAmount = 5;
    protected Rigidbody2D characterBody;
    protected Vector2 velocity;
    protected int speed = 100;
    protected bool isGrounded;
    private SpriteRenderer spriteRenderer;
    private string groundLayer = "Floor";
    protected bool canMove = true;
    private Color originalColor = Color.white;
    protected PlayerMovement playerMovement;

    //Health Bar
    GameObject canvasGameObject;
    GameObject healthBarGameObject;
    GameObject bgImageGameObject;
    GameObject healthBarRectangleGameObject;
    GameObject sliderGameObject;
    Canvas canvas;
    Image healthBarImage;
    Image bgImage;
    Slider slider;
    GameObject sliderObject;
    private bool hasTakenDamage = false;
    protected float health;
    protected float maxHealth = 100;


    protected virtual void Start()
    {
        health = maxHealth;
        velocity = new Vector2(speed, speed);
        characterBody = GetComponent<Rigidbody2D>();
        playerMovement = Player.GetComponent<PlayerMovement>();


        canvasGameObject = new GameObject("EnemyCanvas");
        canvas = canvasGameObject.AddComponent<Canvas>();
        canvasGameObject.transform.SetParent(transform);
        canvasGameObject.transform.localPosition = new Vector3(0, 2, 0); 
    }

    protected virtual void Update()
    {
        FacePlayer();
        if (canMove)
        {
            enemyMove();
        }
    }



    public virtual void TakeDamage(int damage, Vector3 playerPosition)
    {
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
            bgImageRect.anchoredPosition = new Vector2(0, -1f);  
            bgImageRect.anchorMin = new Vector2(0.5f, 0.5f); 
            bgImageRect.anchorMax = new Vector2(0.5f, 0.5f);  
            bgImageRect.pivot = new Vector2(0.5f, 0.5f);     

            bgImage.sprite = sprite;
            bgImage.color = Color.black;

            //Health Bar Image
            healthBarImage = healthBarGameObject.AddComponent<UnityEngine.UI.Image>();

            RectTransform healthBarRect = healthBarGameObject.GetComponent<RectTransform>();
            healthBarRect.sizeDelta = new Vector2(1.5f, 0.1f); 
            healthBarRect.anchoredPosition = new Vector2(0, -1f);  
            healthBarRect.anchorMin = new Vector2(0.5f, 0.5f); 
            healthBarRect.anchorMax = new Vector2(0.5f, 0.5f);  
            healthBarRect.pivot = new Vector2(0.5f, 0.5f);     
           
            healthBarImage.sprite = sprite;
            healthBarImage.color = Color.red;
            healthBarImage.type = Image.Type.Filled;
            healthBarImage.fillMethod = Image.FillMethod.Horizontal;
            healthBarImage.fillOrigin = 0; 
            healthBarImage.fillAmount = 1f;  
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        health -= damage;


        float healthPercentage = health / maxHealth;

        healthBarImage.fillAmount = healthPercentage;




        if (health <= 0)
        {
            Die();
        }

        spriteRenderer.color = Color.red;
        float playerPositionX = playerPosition.x;
        float enemyPositionX = transform.position.x;

        Rigidbody2D characterBody = GetComponent<Rigidbody2D>();
        characterBody.AddForce(Vector2.up * knockbackAmount, ForceMode2D.Impulse);
        if (playerPositionX >= enemyPositionX)
        {
            spriteRenderer.color = Color.red;
            characterBody.AddForce(Vector2.left * knockbackAmount , ForceMode2D.Impulse);
        }
        else
        {
            spriteRenderer.color = Color.red;
            characterBody.AddForce(Vector2.right * knockbackAmount, ForceMode2D.Impulse);
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

    void FacePlayer(){
        //Make Direction of travel towards player
        if (Player != null)
        {
            Vector2 PlayerPosition = Player.transform.position;
            Vector2 EnemyyPosition = transform.position;

            float PlayerX = PlayerPosition.x;
            float PlayerY = PlayerPosition.x;
            float EnemyX = EnemyyPosition.x;
            float EnemyY = EnemyyPosition.x;

            //Rightward Direction
            if (PlayerX >= EnemyX){
                direction = 1;
            }
            //Leftward Direction
            else{
                direction = -1;
            }
        }
    }

    void enemyMove(){

            if(direction == 1){
                characterBody.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
            }
            else{
                characterBody.AddForce(Vector2.left * speed, ForceMode2D.Impulse);
            }
    }

}

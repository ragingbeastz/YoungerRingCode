using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.XR.Interaction;

public class SquareMovement : MonoBehaviour
{
    private Rigidbody2D characterBody;
    private Vector2 velocity;
    public GameObject Player;
    public UnityEngine.UI.Image playerHealth;
    private float lastMovement = 0f;
    private float lastHit = 0f;
    private bool touchingPlayer = false;
    private int direction;
    public int speed = 8;
    public int bounceAmount = 5;

    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector2(speed, speed);
        characterBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float thisMovement = Time.time;
        
        //Make Direction of travel towards player
        if (Player != null)
        {
            Vector2 PlayerPosition = Player.transform.position;
            Vector2 SquarePosition = transform.position;

            float PlayerX = PlayerPosition.x;
            float PlayerY = PlayerPosition.x;
            float SquareX = SquarePosition.x;
            float SquareY = SquarePosition.x;

            //Rightward Direction
            if (PlayerX >= SquareX){
                direction = 1;
            }
            //Leftward Direction
            else{
                direction = -1;
            }

            // Debug.Log("Player X: " + PlayerX + "| Square X: " + SquareX + "| Difference: " + (PlayerX - SquareX));
            float difference = PlayerX - SquareX;
            Debug.Log(difference); 
            if (difference <= 0.5 && difference >= -0.5){
                touchingPlayer = true;
            }
            else{
                touchingPlayer = false;
            }

            Debug.Log(touchingPlayer);
        }

        if ((thisMovement - lastMovement) >= 3f){
            characterBody.AddForce(Vector2.up * bounceAmount, ForceMode2D.Impulse);
            if(direction == 1){
                characterBody.AddForce(Vector2.right * bounceAmount, ForceMode2D.Impulse);
            }
            else{
                characterBody.AddForce(Vector2.left * bounceAmount, ForceMode2D.Impulse);
            }
            
            lastMovement = thisMovement;
        }

        if (thisMovement - lastHit >= 1f){
            if (touchingPlayer){
                Debug.Log("Collision");
                playerHealth.fillAmount -= 0.1f;
                lastHit = thisMovement;
            }
        }
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     // Check if the player collides with the ground layer
    //     if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
    //     {
    //         touchingPlayer = true;
    //     }
    // }

    // void OnCollisionExit2D(Collision2D collision)
    // {
    //     // Check if the player exits collision with the ground layer
    //     if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
    //     {
    //         touchingPlayer = false;
    //     }
    // }
}



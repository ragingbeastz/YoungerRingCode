using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareMovement : MonoBehaviour
{
    private Rigidbody2D characterBody;
    private Vector2 velocity;
    public GameObject Player;
    private float lastMovement = 0f;
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
            float SquareX = SquarePosition.x;

            //Rightward Direction
            if (PlayerX >= SquareX){
                direction = 1;
            }
            //Leftward Direction
            else{
                direction = -1;
            }

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
    }
}

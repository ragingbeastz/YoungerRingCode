using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.XR.Interaction;
using Unity.VisualScripting;

public class SquareMovement : Enemy
{
    public Animator animator;
    protected float lastMovement = 0f;
    private float lastHit = 0f;
    private bool touchingPlayer = false;
    public int bounceAmount = 5;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start(); // Call the parent class's Start() method
        speed = 8;
        canMove = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //base.Update(); // Call the parent class's Update() method

        //Make Direction of travel towards player
        if (Player != null)
        {
            Vector2 PlayerPosition = Player.transform.position;
            Vector2 SquarePosition = transform.position;

            float PlayerX = PlayerPosition.x;
            float PlayerY = PlayerPosition.x;
            float SquareX = SquarePosition.x;
            float SquareY = SquarePosition.x;


            // Debug.Log("Player X: " + PlayerX + "| Square X: " + SquareX + "| Difference: " + (PlayerX - SquareX));
            float difference = PlayerX - SquareX;
            if (difference <= 0.5 && difference >= -0.5)
            {
                touchingPlayer = true;
            }
            else
            {
                touchingPlayer = false;
            }

            AttackPlayer();

        }

    }

    public override void AttackPlayer()
    {
        Vector2 PlayerPosition = Player.transform.position;
        Vector2 SquarePosition = transform.position;

        float PlayerX = PlayerPosition.x;
        float PlayerY = PlayerPosition.x;
        float SquareX = SquarePosition.x;
        float SquareY = SquarePosition.x;
        float thisMovement = Time.time;
        if ((thisMovement - lastMovement) >= 3f)
        {
            characterBody.AddForce(Vector2.up * bounceAmount, ForceMode2D.Impulse);
            if (direction == 1)
            {
                characterBody.AddForce(Vector2.right * bounceAmount, ForceMode2D.Impulse);
            }
            else
            {
                characterBody.AddForce(Vector2.left * bounceAmount, ForceMode2D.Impulse);
            }

            lastMovement = thisMovement;
        }

        if (thisMovement - lastHit >= 1f)
        {
            if (touchingPlayer)
            {
                playerMovement.DamagePlayer(0.1f, SquarePosition);
                lastHit = thisMovement;
            }
        }
    }

    override protected void Die()
    {
        StartCoroutine(WaitFor());
    }

    IEnumerator WaitFor()
    {
        animator.SetFloat("isDead", 1);
        characterBody.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }


}





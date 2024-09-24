using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : Enemy
{
    Animator animator;
    private float lastHit = 0f;
    private float lastPosition = 0f;
    private bool justHit = false;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        Debug.Log(justHit);
        if (transform.position.x - lastPosition != 0 && !justHit)
        {
            lastPosition = transform.position.x;
            isMoving = true;
        }
        else
        {
            isMoving = false;
            justHit = false;
        }

        if (isMoving)
        {
            animator.SetFloat("isMoving", 1);
        }
        else
        {
            animator.SetFloat("isMoving", 0);
        }

        AttackPlayer();

    }

    public override void AttackPlayer()
    {
        float currentHit = Time.time;
        Debug.Log("Current Hit: " + currentHit + " | Last Hit: " + lastHit);
        if (currentHit - lastHit > 1f)
        {
            if (Math.Abs(playerMovement.transform.position.x - transform.position.x) < 2)
            {
                StartCoroutine(HitPlayer());
                lastHit = Time.time;
            }
        }

    }

    private IEnumerator HitPlayer()
    {
        justHit = true;
        canMove = false;
        animator.SetFloat("isHitting", 1);
        characterBody.velocity = new Vector2(0, characterBody.velocity.y);
        playerMovement.DamagePlayer(0.3f, transform.position);
        yield return new WaitForSeconds(0.3f);
        animator.SetFloat("isHitting", 0);
        yield return new WaitForSeconds(1f);
        canMove = true;
    }
}

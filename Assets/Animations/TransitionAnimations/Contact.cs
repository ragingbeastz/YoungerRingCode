using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Contact : MonoBehaviour
{
    public GameObject player;
    protected PlayerMovement playerMovement;
    public UnityEngine.UI.Image transitionImage;
    public float transitionDuration = 5.0f; // Initialize with a default value
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Math.Abs(transform.position.x - player.transform.position.x) <= 0.1 && Math.Abs(transform.position.y - player.transform.position.y) <= 3)
        {
            playerMovement.invincible = true;
            playerMovement.enabled = false;
            playerMovement.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Animator playerAnimator = player.GetComponentInChildren<Animator>();
            playerAnimator.SetFloat("isMoving", 0);
            playerAnimator.SetFloat("isJumping", 0);
            playerAnimator.SetFloat("isRolling", 0);
            playerAnimator.SetFloat("isAttacking1", 0);
            playerAnimator.SetFloat("isAttacking2", 0);
            playerAnimator.SetFloat("isPotion", 0);
            playerAnimator.SetFloat("isHit", 0);

            StartCoroutine(FadeAndLoadScene());
        }

    }

    private IEnumerator FadeAndLoadScene()
    {

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the Orc GameObject.");
        }
        else
        {
            AudioClip hitClip = Resources.Load<AudioClip>("Enemies/Boss/Portal");
            if (hitClip == null)
            {
                Debug.LogError("Failed to load audio clip");
            }
            else
            {
                audioSource.PlayOneShot(hitClip);
            }
        }

        float elapsedTime = 0f;
        transitionImage.gameObject.SetActive(true);
        Color color = transitionImage.color;
        color.a = 0f; // Start with transparent
        transitionImage.color = color;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / transitionDuration);
            transitionImage.color = new Color(color.r, color.g, color.b, alpha);
            Debug.Log(transitionImage.color);
            yield return null;
        }

        SceneManager.LoadScene("BossLevel");
    }
}
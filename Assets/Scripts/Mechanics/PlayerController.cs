using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// This is the main class used to implement control of the player.
/// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public AudioClip jumpAudio;
    public AudioClip respawnAudio;
    public AudioClip ouchAudio;

    /// <summary>
    /// Max horizontal speed of the player.
    /// </summary>
    public float maxSpeed = 7;
    /// <summary>
    /// Initial jump velocity at the start of a jump.
    /// </summary>
    public float jumpTakeOffSpeed = 7;

    //public JumpState jumpState = JumpState.Grounded;
    private bool stopJump;
    /*internal new*/ public Collider2D collider2d;
    /*internal new*/ public AudioSource audioSource;
    public Health health;
    public bool controlEnabled = true;

    bool jump;
    Vector2 move;
    SpriteRenderer spriteRenderer;
    //internal Animator animator;
    readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

    public float moveSpeed = 5f;
    public float runMultiplier = 1.5f;
    float currentMoveSpeed;
    public static PlayerController current;

    public Bounds Bounds => collider2d.bounds;

    public float currentConfidence = 10f;
    public float startConfidence = 10f;
    public Image confidenceImage;
    public TMP_Text confidenceProcentage;

    public GameObject WinLossScreen;
    public TMP_Text WinLossScreenText;

    public GameObject DeathFX;

    void Awake()
    {
        current = this;
        currentConfidence = startConfidence;
        UpdateConfidenceUI();
        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
        collider2d = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
    }

    bool isFrozen = false;
    public void Freeze()
    {
        isFrozen = true;
    }

    public void UnFreeze()
    {
        isFrozen = false;
    }

    private void UpdateConfidenceUI()
    {
        confidenceImage.fillAmount = currentConfidence / 100;
        confidenceProcentage.text = currentConfidence.ToString("F0") + "%";
    }

    void Update()
    {
        currentMoveSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentMoveSpeed = moveSpeed * runMultiplier;
        }

        HandleMovement();
    }

    void HandleMovement()
    {
        //if (!isFrozen) return;

        Vector3 moveDirection = Vector3.zero;

        // Check input and set movement direction
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += Vector3.up; // Move up
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector3.right; // Move right
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDirection += Vector3.down; // Move down
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector3.left; // Move left
        }

        // Normalize the movement direction to ensure consistent speed in all directions
        moveDirection.Normalize();

        // Move the spaceship
        transform.Translate(moveDirection * currentMoveSpeed * Time.deltaTime);
    }

    public void ChangeConfidence(float Amount)
    {
        currentConfidence = Mathf.Clamp(currentConfidence + Amount, 0, 100);
        UpdateConfidenceUI();

        if (currentConfidence <= 0)
        {
            Death();
        }
        else if (currentConfidence >= 100)
        {
            Win();
        }
    }

    bool gameOver = false;
    bool youWin = false;
    private void GameOver()
    {
        if (gameOver) return;
        gameOver = true;
        enabled = false;
        
        StartCoroutine(OpenGameOverScreen());
    }

    IEnumerator OpenGameOverScreen()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;
        WinLossScreen.gameObject.SetActive(true);
        WinLossScreenText.text = "Game Over!";
        yield return null;
    }
    private void Win()
    {
        if (youWin) return;
        youWin = true;
        Time.timeScale = 0f;
        WinLossScreen.gameObject.SetActive(true);
        WinLossScreenText.text = "You Win!";
    }

    public void Death()
    {
        print("player death");
        if (health.IsAlive)
        {
            health.Die();
            model.virtualCamera.m_Follow = null;
            model.virtualCamera.m_LookAt = null;
            // player.collider.enabled = false;
            controlEnabled = false;

            if (audioSource && ouchAudio)
                audioSource.PlayOneShot(ouchAudio);

            GameOver();

            GetComponent<SpriteRenderer>().enabled = false;
            Instantiate(DeathFX, transform.position, Quaternion.identity);

            //animator.SetTrigger("hurt");
            //animator.SetBool("dead", true);
            //Simulation.Schedule<PlayerSpawn>(2);
        }
    }

    public void OnTokenCollision(float amount)
    {
        ChangeConfidence(amount);
    }
}

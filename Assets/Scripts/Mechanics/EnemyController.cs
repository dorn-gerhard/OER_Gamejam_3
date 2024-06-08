using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using Platformer.Mechanics;
using UnityEngine;
using static Platformer.Core.Simulation;


/// <summary>
/// A simple controller for enemies. Provides movement control over a patrol path.
/// </summary>
[RequireComponent(typeof(AnimationController), typeof(Collider2D))]
public class EnemyController : MonoBehaviour
{
    public AudioClip ouch;
    public GameObject Token;

    internal AnimationController control;
    internal Collider2D _collider;
    internal AudioSource _audio;
    SpriteRenderer spriteRenderer;

    public float moveSpeed = 2.5f;
    float currentMoveSpeed;

    public Bounds Bounds => _collider.bounds;

    public float confidenceReductionOnTouch = 1;

    public GameObject DeathFX;

    public Health health;

    void Awake()
    {
        health = GetComponent<Health>();
        control = GetComponent<AnimationController>();
        _collider = GetComponent<Collider2D>();
        _audio = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    bool isFrozen = false;
    public void Freeze()
    {
        isFrozen = true;
    }

    public void Unfreeze()
    {
        isFrozen = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            if (health.IsAlive)
            {
                player.ChangeConfidence(-confidenceReductionOnTouch);
                Destroy(gameObject);
            }
            else
            {
                player.OnTokenCollision(1);
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        var projectile = collider.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            health.Decrement();
            //Destroy(projectile.gameObject);
        }
    }

    void Update()
    {
        currentMoveSpeed = moveSpeed;

        HandleMovement();
    }

    public void Death()
    {
        //if (enemy._audio && enemy.ouch)
        //    enemy._audio.PlayOneShot(enemy.ouch);

        //Instantiate(Token, transform.position, Quaternion.identity);

        ////spawn death fx
        //Destroy(gameObject);

        GetComponent<SpriteRenderer>().color = Color.green;
        //var deathFX = Instantiate(DeathFX, transform.position, Quaternion.identity);
        //deathFX.transform.localScale *= 0.5f;
    }

    void HandleMovement()   
    {
        //if (isFrozen) return;
        if (!health.IsAlive) return;

        Vector3 moveDirection = PlayerController.current.transform.position - transform.position;

        // Normalize the movement direction to ensure consistent speed in all directions
        moveDirection.Normalize();

        // Move the spaceship
        transform.Translate(moveDirection * currentMoveSpeed * Time.deltaTime);
    }

}

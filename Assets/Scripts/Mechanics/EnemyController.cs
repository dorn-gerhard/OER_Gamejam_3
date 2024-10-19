using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using Platformer.Mechanics;
using TMPro;
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
    public float confidenceGain = 5f;
    float currentMoveSpeed;

    public Bounds Bounds => _collider.bounds;

    public float confidenceReductionOnTouch = 1;

    public GameObject DeathFX;

    public Health health;

    public GameObject inSightsVisuals;
    public GameObject correctFunctionVisuals;
    public GameObject wrongFunctionVisuals;


    bool isFrozen = false;
    public bool isInSights = false;
    public int combo;

    void Awake()
    {
        health = GetComponent<Health>();
        control = GetComponent<AnimationController>();
        _collider = GetComponent<Collider2D>();
        _audio = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

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
                player.ChangeConfidence(-confidenceReductionOnTouch, health.functionType);
                player.PlaySound("ouch");
                Destroy(gameObject);
            }
            else
            {
                player.ChangeConfidence(confidenceGain, health.functionType);
                player.PlaySound("collectable");                
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        var projectile = collider.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            health.Decrement(GetComponent<Health>().functionType);
            //Destroy(projectile.gameObject);
        }
    }

    void Update()
    {
        currentMoveSpeed = moveSpeed;

        HandleMovement();

        bool firstCall = inSightsVisuals.activeSelf == false;
        if (isInSights)
        {
            inSightsVisuals.SetActive(true);

            if (health.functionType == PlayerController.current.GetCurrentFunctionScriptableObject().type)
            {
                wrongFunctionVisuals.SetActive(false);

                if (firstCall)
                {
                    PlayerController.current.PlaySound("targeting");
                }

                correctFunctionVisuals.SetActive(true);


                var tmp_text = correctFunctionVisuals.GetComponent<TMP_Text>();

                switch(combo)
                {
                    case 0:
                        break;

                    case 1:
                        break;

                    case 2:
                        tmp_text.text = "Doppeltreffer!";
                        break;

                    case 3:
                        tmp_text.text = "Dreifachtreffer!";
                        break;

                    case 4:
                        tmp_text.text = "Vierfachtreffer!";
                        break;

                    case 5:
                        tmp_text.text = "Fünffachtreffer!";
                        break;

                    case 6:
                        tmp_text.text = "Sechsfachtreffer!";
                        break;

                    case 7:
                        tmp_text.text = "Achtfachtreffer!";
                        break;

                    case 9:
                        tmp_text.text = "Neunfachtreffer!";
                        break;

                    case 10:
                        tmp_text.text = "Zehnfachtreffer!";
                        break;

                    default:
                        tmp_text.text = "Kombotreffer x" + combo;
                        break;
                }
                
            }
            else
            {
                correctFunctionVisuals.SetActive(false);

                if (firstCall)
                {
                    PlayerController.current.PlaySound("error");
                }

                wrongFunctionVisuals.SetActive(true);
            }

            isInSights = false;
        }
        else
        {
            inSightsVisuals.SetActive(false);
        }
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

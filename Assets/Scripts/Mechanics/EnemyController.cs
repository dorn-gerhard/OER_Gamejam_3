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

        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;

        public float moveSpeed = 2.5f;
        float currentMoveSpeed;

        public Bounds Bounds => _collider.bounds;

        void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.health.Decrement();
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


            //spawn death fx
            Destroy(gameObject);
        }

        void HandleMovement()   
        {
            Vector3 moveDirection = PlayerController.current.transform.position - transform.position;

            // Normalize the movement direction to ensure consistent speed in all directions
            moveDirection.Normalize();

            // Move the spaceship
            transform.Translate(moveDirection * currentMoveSpeed * Time.deltaTime);
        }

    }

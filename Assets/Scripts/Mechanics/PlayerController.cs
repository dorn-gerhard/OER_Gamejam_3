using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using Platformer.Mechanics;

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
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public float moveSpeed = 5f;
        public float runMultiplier = 1.5f;
        float currentMoveSpeed;
        public static PlayerController current;

        public Bounds Bounds => collider2d.bounds;

        void Awake()
        {
            current = this;
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
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
        //public enum JumpState
        //{
        //    Grounded,
        //    PrepareToJump,
        //    Jumping,
        //    InFlight,
        //    Landed
        //}

        public void Death()
        {
            print("player death");
            var player = model.player;
            if (player.health.IsAlive)
            {
                player.health.Die();
                model.virtualCamera.m_Follow = null;
                model.virtualCamera.m_LookAt = null;
                // player.collider.enabled = false;
                player.controlEnabled = false;

                if (player.audioSource && player.ouchAudio)
                    player.audioSource.PlayOneShot(player.ouchAudio);
                player.animator.SetTrigger("hurt");
                player.animator.SetBool("dead", true);
                //Simulation.Schedule<PlayerSpawn>(2);
            }
        }
    }

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Code
{
    public class CharacterController : MonoBehaviour
    {
        [Inject] private Model _model;
        [Inject] private DiContainer _diContainer;
        
        [SerializeField] private Rigidbody2D rigidbody2D;
        [SerializeField] private Transform bottomOfCharacter;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private Collider2D encounterTrigger;
        [SerializeField] private GameObject attackPrefab;
        [SerializeField] private Transform attackReadyParent;

        private bool _jumpInProgress;
        private float _jumpedSoFar;

        private bool IsGrounded()
        {
            var hit = Physics2D.Raycast(bottomOfCharacter.position, Vector2.down, 0.01f, LayerMask.GetMask("Ground"));
            return hit.collider != null;
        }

        private void Start()
        {
            encounterTrigger.OnTriggerEnter2DAsObservable().Select(x => x.GetComponent<EnemyController>()).Subscribe(async enemy =>
            {
                if (enemy == null) return;
                _model.inputEnabled = false;
                var correct = await enemy.TriggerProblem();
                var attack = _diContainer.InstantiatePrefab(attackPrefab, attackReadyParent).GetComponent<AttackVisualsController>();
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                if (correct)
                {
                    await attack.Fly(enemy.transform.position);
                    attack.Explode();
                    await enemy.Die();
                }
                else
                {
                    await attack.Fly(enemy.transform.position);
                    await attack.Fly(transform.position);
                    attack.Explode();
                    _model.health--;
                    rigidbody2D.simulated = false;
                    await transform.DOMove(enemy.LosePosition.position, .5f).SetEase(Ease.Flash).AsyncWaitForCompletion();
                    rigidbody2D.simulated = true;
                    await enemy.TriggerExplanation();
                }
                _model.inputEnabled = true;

                if (_model.health <= 0)
                {
                    _model.health = 0;
                    SceneManager.LoadScene("Main");
                }
            }).AddTo(this);
        }

        private void FixedUpdate()
        {
            var dx = Vector2.zero;

            if (!_model.inputEnabled)
            {
                _model.horizontalInput = 0;
                _model.isJumping = false;
                _model.isDropping.Value = false;
            }
            
            if (_model.horizontalInput != 0)
            {
                var dh = Vector2.right * (_model.horizontalInput * _model.horizontalSpeed);
                dx += dh;
                
                spriteRenderer.flipX = _model.horizontalInput < 0;
            }

            var grounded = IsGrounded();
            
            if (grounded && _model.isJumping && !_jumpInProgress)
            {
                _jumpInProgress = true;
                _jumpedSoFar = 0;
            }

            if (_jumpInProgress)
            {
                var dv = Vector2.up * _model.jumpSpeed;
                dx += dv;

                if (!_model.isJumping && _jumpedSoFar >= _model.minJumpHeight)
                    _jumpInProgress = false;

                if (_model.isJumping && _jumpedSoFar >= _model.maxJumpHeight)
                    _jumpInProgress = false;
            }
            else if(!grounded)
            {
                var dv = Vector2.down * _model.dropSpeed;
                dx += dv;
            }

            dx *= Time.fixedDeltaTime;
            rigidbody2D.position += dx;
            _jumpedSoFar += dx.y;
            
            if(dx.x != 0)
                animator.Play("Run");
            else
                animator.Play("Idle");

            _model.playerY = bottomOfCharacter.position.y;
        }
    }
}
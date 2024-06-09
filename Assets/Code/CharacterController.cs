using UnityEngine;
using Zenject;

namespace Code
{
    public class CharacterController : MonoBehaviour
    {
        [Inject] private Model _model;
        
        [SerializeField] private Rigidbody2D rigidbody2D;
        [SerializeField] private Transform bottomOfCharacter;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;

        private bool _jumpInProgress;
        private float _jumpedSoFar;

        private bool IsGrounded()
        {
            var hit = Physics2D.Raycast(bottomOfCharacter.position, Vector2.down, 0.01f);
            return hit.collider != null;
        }
        
        private void FixedUpdate()
        {
            var dx = Vector2.zero;
            
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
        }
    }
}
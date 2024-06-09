using System;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code
{
    public class CharacterInputController : MonoBehaviour
    {
        [Inject] private Model _model;

        [SerializeField] private bool useKeyboard;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button jumpButton;
        [SerializeField] private Button dropDownButton;
        
        private struct PlayerHorizontalInput
        {
            public bool left;
            public bool right;
        }
        
        private struct PlayerVerticalInput
        {
            public bool up;
            public bool down;
        }

        private class PlayerInput
        {
            public PlayerHorizontalInput horizontal { get; set; }
            public PlayerVerticalInput vertical { get; set; }

            public PlayerInput()
            {
                horizontal = default;
                vertical = default;
            }
        }

        private PlayerInput _input;
        
        private void Start()
        {
            _input = new PlayerInput();
            
            var ld = leftButton.OnPointerDownAsObservable().Subscribe(_ =>
            {
                var h = _input.horizontal;
                h.left = true;
                _input.horizontal = h;
            }).AddTo(this);
            var lu = leftButton.OnPointerUpAsObservable().Subscribe(_ =>
            {
                var h = _input.horizontal;
                h.left = false;
                _input.horizontal = h;
            }).AddTo(this);
            var rd = rightButton.OnPointerDownAsObservable().Subscribe(_ =>
            {
                var h = _input.horizontal;
                h.right = true;
                _input.horizontal = h;
            }).AddTo(this);
            var ru = rightButton.OnPointerUpAsObservable().Subscribe(_ =>
            {
                var h = _input.horizontal;
                h.right = false;
                _input.horizontal = h;
            }).AddTo(this);

            Observable.EveryValueChanged(_input, input => input.horizontal).Subscribe(h =>
            {
                if (h.left)
                    _model.horizontalInput = -1f;
                else if (h.right)
                    _model.horizontalInput = 1f;
                else
                    _model.horizontalInput = 0f;
            }).AddTo(this);
            
            var ud = jumpButton.OnPointerDownAsObservable().Subscribe(_ =>
            {
                var v = _input.vertical;
                v.up = true;
                _input.vertical = v;
            }).AddTo(this);
            var uu = jumpButton.OnPointerUpAsObservable().Subscribe(_ =>
            {
                var v = _input.vertical;
                v.up = false;
                _input.vertical = v;
            }).AddTo(this);
            var dd = dropDownButton.OnPointerDownAsObservable().Subscribe(_ =>
            {
                var v = _input.vertical;
                v.down = true;
                _input.vertical = v;
            }).AddTo(this);
            var du = dropDownButton.OnPointerUpAsObservable().Subscribe(_ =>
            {
                var v = _input.vertical;
                v.down = false;
                _input.vertical = v;
            }).AddTo(this);
            
            Observable.EveryValueChanged(_input, input => input.vertical).Subscribe(v =>
            {
                _model.isJumping = v.up;
                _model.isDropping.Value = v.down;
            }).AddTo(this);

            Observable.EveryValueChanged(_model, model => model.inputEnabled).Subscribe(inputOn =>
            {
                jumpButton.gameObject.SetActive(inputOn);
                dropDownButton.gameObject.SetActive(inputOn);
                leftButton.gameObject.SetActive(inputOn);
                rightButton.gameObject.SetActive(inputOn);
            }).AddTo(this);
        }

        private void Update()
        {
            if (useKeyboard)
            {
                _input.horizontal = new PlayerHorizontalInput
                {
                    left = Input.GetKey(KeyCode.A),
                    right = Input.GetKey(KeyCode.D)
                };
            
                _input.vertical = new PlayerVerticalInput()
                {
                    up = Input.GetKey(KeyCode.W),
                    down = Input.GetKey(KeyCode.S)
                };
            }
        }
    }
}
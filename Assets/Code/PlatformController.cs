using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

namespace Code
{
    public class PlatformController : MonoBehaviour
    {
        [Inject] private Model _model;
        
        [SerializeField] private Collider2D _collider;

        private bool _isDeactivated;
        
        private void Start()
        {
            _model.isDropping.AsObservable().DistinctUntilChanged().Subscribe(async isDropping =>
            {
                if (_isDeactivated || !isDropping) return;
                _isDeactivated = true;
                _collider.enabled = false;
                await UniTask.Delay(TimeSpan.FromSeconds(_model.platformDeactivationDuration));
                _collider.enabled = true;
                _isDeactivated = false;
            }).AddTo(this);
        }

        private void Update()
        {
            if (_isDeactivated) return;
            _collider.enabled = _model.playerY >= transform.position.y;
        }
    }
}
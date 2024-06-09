using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using UnityEngine;

namespace Code
{
    public class AttackVisualsController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem baseParticles;
        [SerializeField] private ParticleSystem explodeParticles;
        
        public async void Explode()
        {
            baseParticles.Stop();
            explodeParticles.Play();
            await explodeParticles.OnDestroyAsync();
            Destroy(gameObject);
        }

        public async UniTask Fly(Vector3 target)
        {
            await transform.DOMove(target, .5f).SetEase(Ease.Flash).AsyncWaitForCompletion();
        }
    }
}
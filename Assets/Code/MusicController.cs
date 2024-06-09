using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using Zenject;

namespace Code
{
    public class MusicController : MonoBehaviour
    {
        [Inject] private Model _model;

        [SerializeField] private AudioSource src;
        [SerializeField] private AudioClip bg;
        [SerializeField] private AudioClip fight;
        
        private void Start()
        {
            Observable.EveryValueChanged(_model, model => model.inputEnabled).DistinctUntilChanged().Subscribe(async notInAFight =>
                {
                    var to = notInAFight ? bg : fight;
                    await src.DOFade(0, 3f).AsyncWaitForCompletion();
                    src.clip = to;
                    src.Play();
                    await src.DOFade(1f, 3f).AsyncWaitForCompletion();
                }).AddTo(this);
        }
    }
}
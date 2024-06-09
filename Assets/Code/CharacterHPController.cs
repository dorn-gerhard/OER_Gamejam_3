using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

namespace Code
{
    public class CharacterHPController : MonoBehaviour
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private Model _model;
        
        [SerializeField] private Transform hpParent;
        [SerializeField] private GameObject hpPrefab;

        private void Start()
        {
            Observable.EveryValueChanged(_model, model => model.health).Subscribe(hp =>
            {
                for (int ch = hpParent.childCount -1 ; ch >=0 ; --ch)
                {
                    var child = hpParent.GetChild(ch);
                    Destroy(child.gameObject);
                }

                for (int h = 0; h < hp; ++h)
                {
                    _diContainer.InstantiatePrefab(hpPrefab, hpParent);
                }
            }).AddTo(this);
        }
    }
}
using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code
{
    public class EnemyController : MonoBehaviour
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private ScreenRoot _screenRoot;
        
        [SerializeField] private string difficulty;
        [SerializeField] private int termCount;
        [SerializeField] private int minNumber;
        [SerializeField] private int maxNumber;
        [SerializeField] private bool nominatorLargerThanDenominator;
        [SerializeField] private GameObject problemControllerPrefab;
        [SerializeField] private GameObject problemExplanationPrefab;
        [SerializeField] private GameObject surpriseVisualsPrefab;
        [SerializeField] private Transform surpriseParent;
        [SerializeField] private Animator animator;
        
        public Transform LosePosition;

        private MathProblem _problem;

        public async UniTask<bool> TriggerProblem()
        {
            var surprise = _diContainer.InstantiatePrefab(surpriseVisualsPrefab, surpriseParent);
            await surprise.OnDestroyAsync();
            _problem = MathProblem.GenerateFractionsProblem(difficulty, termCount, minNumber, maxNumber, nominatorLargerThanDenominator);
            var problemController = _diContainer.InstantiatePrefab(problemControllerPrefab, _screenRoot.transform).GetComponent<FractionsProblemScreenController>();
            var correct = await problemController.Init(_problem);
            Destroy(problemController.gameObject);
            return correct;
        }

        public async UniTask TriggerExplanation()
        {
            var problemExplanation = _diContainer.InstantiatePrefab(problemExplanationPrefab, _screenRoot.transform).GetComponent<ProblemExplanationScreenController>();
            await problemExplanation.Init(_problem);
            Destroy(problemExplanation.gameObject);
        }

        public async UniTask Die()
        {
            animator.Play("Die");
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            Destroy(gameObject);
        }
    }
}
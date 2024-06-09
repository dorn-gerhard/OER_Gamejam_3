using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Code
{
    public class EnemyEncounterTestController : MonoBehaviour
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private ScreenRoot _screenRoot;
        
        [SerializeField] private GameObject problemControllerPrefab;
        [SerializeField] private GameObject problemExplanationPrefab;
        
        public async UniTask Init()    
        {
            var problem = MathProblem.GenerateFractionsProblem(MathDifficulties.EzDifficulty, 3, 2, 10, false);
            var problemController = _diContainer.InstantiatePrefab(problemControllerPrefab, _screenRoot.transform).GetComponent<FractionsProblemScreenController>();
            var correct = await problemController.Init(problem);
            Destroy(problemController.gameObject);
            if (!correct)
            {
                var problemExplanation = _diContainer.InstantiatePrefab(problemExplanationPrefab, _screenRoot.transform).GetComponent<ProblemExplanationScreenController>();
                await problemExplanation.Init(problem);
                Destroy(problemExplanation.gameObject);
            }
        }
    }
}
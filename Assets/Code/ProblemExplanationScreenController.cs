using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Code
{
    public class ProblemExplanationScreenController : MonoBehaviour
    {
        [SerializeField] private MathProblemVisualizationController problemLabel;
        [SerializeField] private Transform stepsParent;
        [SerializeField] private GameObject stepPrefab;
        [SerializeField] private Button okButton;

        public async UniTask Init(MathProblem problem)
        {
            problemLabel.Init(problem);

            var steps = problem.GetStepwiseExplanation();
            for (int s = 0; s < steps.Count; ++s)
            {
                Instantiate(stepPrefab, stepsParent).GetComponent<ExplanationStepController>().Init(steps[s]);
            }

            await okButton.OnClickAsync();
        }
    }
}
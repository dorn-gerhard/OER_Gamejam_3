using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Code
{
    public class FractionsProblemScreenController : MonoBehaviour
    {
        [SerializeField] private MathProblemVisualizationController problemLabel;
        [SerializeField] private GameObject screenParent;
        [SerializeField] private FractionInputController resultInput;
        [SerializeField] private Button okButton;

        // returns true if answered correctly
        public UniTask<bool> Init(MathProblem problem)
        {
            var src = new UniTaskCompletionSource<bool>();
            
            problemLabel.Init(problem);

            okButton.OnClickAsObservable().Subscribe(_ =>
            {
                var input = resultInput.GetInput();
                var expected = problem.Evaluate();
                src.TrySetResult(input.Equals(expected));
                screenParent.SetActive(false);
            }).AddTo(this);
            
            screenParent.SetActive(true);
            return src.Task;
        }
    }
}
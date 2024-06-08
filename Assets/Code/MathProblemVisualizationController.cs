using UnityEngine;

namespace Code
{
    public class MathProblemVisualizationController : MonoBehaviour
    {
        [SerializeField] private Transform problemParent;
        [SerializeField] private GameObject termPrefab;
        [SerializeField] private GameObject signPrefab;

        public void Init(MathProblem problem)
        {
            for (int t = 0; t < problem.terms.Length; ++t)
            {
                var term = problem.terms[t];
                if(t > 0)
                    Instantiate(signPrefab, problemParent).GetComponent<SignTermController>().Init(term);
                Instantiate(termPrefab, problemParent).GetComponent<FractionsTermController>().Init(term, t == 0);
            }
        }
    }
}
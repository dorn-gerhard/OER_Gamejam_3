using TMPro;
using UnityEngine;

namespace Code
{
    public class ExplanationStepController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI stepLabel;

        public void Init(string step)
        {
            stepLabel.text = step;
        }
    }
}
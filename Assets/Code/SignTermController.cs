using TMPro;
using UnityEngine;

namespace Code
{
    public class SignTermController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI signLabel;

        public void Init(MathValue value)
        {
            signLabel.text = value.nominator >= 0 ? "+" : "-";
        }
    }
}
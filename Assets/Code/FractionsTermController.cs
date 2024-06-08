using TMPro;
using UnityEngine;

namespace Code
{
    public class FractionsTermController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI wholeLabel;
        [SerializeField] private GameObject fractionGroup;
        [SerializeField] private TextMeshProUGUI nominatorLabel;
        [SerializeField] private TextMeshProUGUI denominatorLabel;
        [SerializeField] private TextMeshProUGUI signLabel;

        public void Init(MathValue value, bool showSign)
        {
            var whole = Mathf.Abs(value.GetWhole());
            if(whole != 0)
                wholeLabel.text = whole.ToString();
            if (value.GetNominatorRemainder() == 0)
            {
                fractionGroup.SetActive(false);
            }
            else
            {
                nominatorLabel.text = Mathf.Abs(value.GetNominatorRemainder()).ToString();
                denominatorLabel.text = value.denominator.ToString();
            }
            if(showSign)
                signLabel.text = value.nominator < 0 ? "-" : "";
        }
    }
}
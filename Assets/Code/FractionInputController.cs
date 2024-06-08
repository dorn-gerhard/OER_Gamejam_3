using TMPro;
using UnityEngine;

namespace Code
{
    public class FractionInputController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField signInput;
        [SerializeField] private TMP_InputField wholeInput;
        [SerializeField] private TMP_InputField nominatorInput;
        [SerializeField] private TMP_InputField denominatorInput;

        public MathValue GetInput()
        {
            int whole;
            if (wholeInput.text == "")
                whole = 0;
            else 
                whole = int.Parse(wholeInput.text);
            int nominator;
            if (nominatorInput.text == "")
                nominator = 0;
            else
                nominator = int.Parse(nominatorInput.text);

            int denominator;
            if (denominatorInput.text == "")
                denominator = 1;
            else
                denominator = int.Parse(denominatorInput.text);
            var sign = signInput.text == "-" ? -1 : 1;

            return new MathValue
            {
                nominator = sign * whole * denominator + nominator,
                denominator = denominator
            };
        }
    }
}
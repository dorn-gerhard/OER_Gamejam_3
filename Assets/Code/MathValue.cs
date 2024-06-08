using UnityEngine;

namespace Code
{
    public struct MathValue
    {
        public int nominator;
        public int denominator;

        public override string ToString()
        {
            var sign = nominator >= 0 ? "" : "-";
            var whole = GetWhole();
            var remainder = GetNominatorRemainder();
            if(remainder == 0)
                return $"{sign}{whole}";
            if(whole == 0)
                return $"{sign}{remainder}/{denominator}";
            return $"{sign}{whole} {remainder}/{denominator}";
        }

        public bool Equals(MathValue other)
        {
            var me = Simplify();
            other = other.Simplify();
            return me.nominator == other.nominator && me.denominator == other.denominator;
        }

        public MathValue Simplify()
        {
            var lcm = MathUseCases.HighestCommonFactor(new int[] { GetNominatorRemainder(), denominator });
            return ChangeDenominator(lcm);
        }

        public int GetNominatorRemainder()
        {
            return nominator % denominator;
        }

        public int GetWhole()
        {
            return Mathf.FloorToInt((float)nominator / denominator);
        }

        public MathValue ChangeDenominator(int newDenominator)
        {
            var differenceFactor = (float)newDenominator / denominator;
            return new MathValue
            {
                nominator = Mathf.RoundToInt(nominator * differenceFactor),
                denominator = newDenominator
            };
        }
    }
}
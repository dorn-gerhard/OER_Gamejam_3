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
            var simplifiedMe = Simplify();
            var simplifiedOther = other.Simplify();
            return simplifiedMe.nominator == simplifiedOther.nominator && 
                   simplifiedMe.denominator == simplifiedOther.denominator;
        }

        public MathValue Simplify()
        {
            var lcm = MathUseCases.HighestCommonFactor(new int[] { nominator, denominator });
            if (lcm == 1) return this;
            var changed = ChangeDenominator(denominator / lcm);
            return changed;
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
            int newNominator;
            var whole = GetWhole();
            var remainder = GetNominatorRemainder();
            if (newDenominator < denominator)
            {
                newNominator = remainder / (denominator / newDenominator);
            }
            else
            {
                newNominator = remainder * (newDenominator / denominator);
            }
            return new MathValue
            {
                nominator = newNominator + whole * newDenominator,
                denominator = newDenominator
            };
        }
    }
}
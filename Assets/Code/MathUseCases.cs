using System.Collections.Generic;
using System.Linq;

namespace Code
{
    public static class MathUseCases
    {
        public static int LeastCommonMultiple(int[] input)
        {
            var factors = new List<int>[input.Length];
            for (int fs = 0; fs < input.Length; ++fs)
            {
                factors[fs] = GetPrimeFactors(input[fs]);
            }

            var commonFactors = new List<int>(factors[0]);
            for (int fs = 1; fs < input.Length; ++fs)
            {
                commonFactors = commonFactors.Common(factors[fs]).Concat(commonFactors.Except(factors[fs]).Union(factors[fs].Except(commonFactors))).ToList();
            }

            if (commonFactors.Count == 0) return 1;

            return commonFactors.Aggregate((r, x) => r * x);
        }

        public static int HighestCommonFactor(int[] input)
        {
            var factors = new List<int>[input.Length];
            for (int fs = 0; fs < input.Length; ++fs)
            {
                factors[fs] = GetPrimeFactors(input[fs]);
            }

            var commonFactors = new List<int>(factors[0]);
            for (int fs = 1; fs < input.Length; ++fs)
            {
                commonFactors = commonFactors.Common(factors[fs]).ToList();
            }

            if (commonFactors.Count == 0) return 1;

            return commonFactors.Aggregate((r, x) => r * x);
        }
        
        public static List<int> GetPrimeFactors(int number)
        {
            if (number == 1) return new List<int>{1};
            var primes = new List<int>();

            for (int div = 2; div <= number; div++)
                while (number % div == 0)
                {
                    primes.Add(div);
                    number /= div;
                }
    
            return primes;
        }
    }
}
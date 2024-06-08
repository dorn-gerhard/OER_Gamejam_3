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
                commonFactors = commonFactors.Union(factors[fs]).ToList();
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
                commonFactors = commonFactors.Intersect(factors[fs]).ToList();
            }

            if (commonFactors.Count == 0) return 1;

            return commonFactors.Aggregate((r, x) => r * x);
        }
        
        public static List<int> GetPrimeFactors(int number)
        {
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
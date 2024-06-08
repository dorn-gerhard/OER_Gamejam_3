using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code
{
    public struct MathProblem
    {
        public string problemType;
        public MathValue[] terms;

        public static MathProblem GenerateFractionsProblem(string difficulty, int termCount, int minNumber, int maxNumber, bool nominatorLargerThanDenominator)
        {
            var problem = new MathProblem();
            problem.terms = new MathValue[termCount];
            problem.problemType = MathProblemTypes.FRACTIONS;
            var commonDenominator = Random.Range(minNumber, maxNumber + 1);

            for (int t = 0; t < termCount; ++t)
            {
                int nominator = 0, denominator = 0;
                switch (difficulty)
                {
                    case MathDifficulties.EzDifficulty:
                        if(nominatorLargerThanDenominator)
                            nominator = Random.Range(commonDenominator, maxNumber + 1);
                        else
                            nominator = Random.Range(minNumber, commonDenominator);
                        denominator = commonDenominator;
                        break;
                    case MathDifficulties.MedDifficulty:
                        break;
                    case MathDifficulties.BossDifficulty:
                        break;
                }
                problem.terms[t] = new MathValue
                {
                    nominator = nominator,
                    denominator = denominator
                };
            }

            return problem;
        }
        
        public MathValue Evaluate()
        {
            MathValue result = default;
            switch (problemType)
            {
                case MathProblemTypes.FRACTIONS:
                    result = EvaluateFractions();
                    break;
            }
            return result;
        }

        private MathValue EvaluateFractions()
        {
            /*
             * 1. find LCM of denominators
             * 2. convert all values to have lcm as the denominator
             * 3. sum values
             */
            var denominators = terms.Select(x => x.denominator).ToArray();
            var denominatorsSame = true;
            for (int d = 1; d < denominators.Length; ++d)
            {
                if (denominators[d - 1] != denominators[d])
                {
                    denominatorsSame = false;
                    break;
                }
            }

            IEnumerable<int> nominators;
            if (denominatorsSame)
            {
                nominators = terms.Select(x => x.nominator);
                return new MathValue
                {
                    nominator = nominators.Aggregate((r, x) => r + x),
                    denominator = denominators[0]
                };
            }
            var lcm = MathUseCases.LeastCommonMultiple(denominators);
            nominators = terms.Select(x => x.ChangeDenominator(lcm).nominator);
            return new MathValue
            {
                nominator = nominators.Aggregate((r, x) => r + x),
                denominator = lcm
            };
        }

        public List<string> GetStepwiseExplanation()
        {
            var result = new List<string>();
            switch (problemType)
            {
                case MathProblemTypes.FRACTIONS:
                    result.Add("Firstly, find the Least Common Multiple of all of the denominators.");
                    result.Add("----------");                    
                    result.Add("To do that, firstly find the Prime Factors of each denominator:");
                    var termFactors = new List<List<int>>();
                    foreach (var term in terms)
                    {
                        var factors = MathUseCases.GetPrimeFactors(term.denominator);
                        termFactors.Add(factors);
                        var factorsString = $"{term}: ";
                        for (int f = 0; f < factors.Count; ++f)
                        {
                            factorsString += factors[f] + ", ";
                        }
                        factorsString = factorsString.TrimEnd(' ', ',');
                        result.Add(factorsString);
                    }
                    result.Add("----------");                    
                    result.Add("Then, find which prime factors are the same across all denominators:");
                    var commonFactors = new List<int>(termFactors[0]);
                    for (int fs = 1; fs < termFactors.Count; ++fs)
                    {
                        commonFactors = commonFactors.Union(termFactors[fs]).ToList();
                    }
                    var commonFactorsString = "";
                    for (int f = 0; f < commonFactors.Count; ++f)
                    {
                        commonFactorsString += commonFactors[f] + ", ";
                    }
                    commonFactorsString = commonFactorsString.TrimEnd(' ', ',');
                    result.Add(commonFactorsString);
                    result.Add("----------");                    
                    result.Add("Lastly, we multiply all of the common factors, and that's our Least Common Multiple");
                    var productOfCommonFactorsString = "";
                    var productOfCommonFactors = 1;
                    for (int f = 0; f < commonFactors.Count; ++f)
                    {
                        productOfCommonFactorsString += commonFactors[f] + " * ";
                        productOfCommonFactors *= commonFactors[f];
                    }
                    productOfCommonFactorsString = productOfCommonFactorsString.TrimEnd(' ', '*');
                    productOfCommonFactorsString += $" = {productOfCommonFactors}";
                    result.Add(productOfCommonFactorsString);
                    result.Add("----------");
                    result.Add("Now that we have the Least Common Multiple, we will use it to convert all fractions to have the same denominator:");
                    foreach (var term in terms)
                    {
                        result.Add($"{term} -> {term.ChangeDenominator(productOfCommonFactors)}");
                    }
                    result.Add("----------");
                    result.Add("Finally, we can add up all the terms and call it a day:");
                    var finalResultString = "";
                    foreach (var term in terms)
                    {
                        finalResultString += $"{term.ChangeDenominator(productOfCommonFactors)} + ";
                    }
                    finalResultString = finalResultString.TrimEnd(' ', '+');
                    finalResultString += $" = {Evaluate()}";
                    result.Add(finalResultString);
                    break;
            }

            return result;
        }
    }
}
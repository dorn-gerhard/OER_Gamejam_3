using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MathFunctionPolynom : MathFunction
{
    public List<float> coefficients; // 0... constant, 1 ... x, 2 ... x^2

    public override Vector3? EvalFunc(float x)
    {
        float result = 0;
        for (int k = 0; k < coefficients.Count; k++)
        {
            result += coefficients[k] * Mathf.Pow(x, k);
        }
        return new Vector3(x, result, 0);
    }

    public override string DescriptionLatex()
    {
        string polynom = "";
        string textTemp = "";
        for (int k = coefficients.Count - 1; k >= 0; k--)
        {
            if (k == 0)
                textTemp = $"{coefficients[k]}";
            else if (k == 1)
            {
                if (coefficients[k] == 1)
                    textTemp = $"x + ";
                else
                    textTemp = $"{coefficients[k]} x + ";
                

            }
            else
            {
                if (coefficients[k] == 1)
                    textTemp = $"x^{k} + ";
                else
                    textTemp = $"{coefficients[k]} x^{k} + ";   
            }

            polynom += textTemp;
        }
        return polynom;
    }


    public override string Description()
    {
        string polynom = "";
        string textTemp = "";
        for (int k = coefficients.Count - 1; k >= 0; k--)
        {
            if (k == 0)
                textTemp = $"{coefficients[k]}";
            else if (k == 1)
            {
                if (coefficients[k] == 1)
                    textTemp = $"x + ";
                else
                    textTemp = $"{coefficients[k]} x + ";


            }
            else
            {
                if (coefficients[k] == 1)
                    textTemp = $"x^{k} + ";
                else
                    textTemp = $"{coefficients[k]} x^{k} + ";
            }

            polynom += textTemp;
        }
        return polynom;
    }
}

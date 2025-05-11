using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MathFunctionLinear : MathFunction
{
    public float d = 1.0f;
    public float k = 1.0f;

    public override Vector3? EvalFunc(float x)
    {
        return new Vector3(x, k * x + d, 0);
    }

    public override string DescriptionLatex()
    {
        return $"y(x) = {k} \\cdot x + {d}";
    }


    public override string Description()
    {
        return $"y(x) = {k} x + {d}";
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MathFunctionCircle : MathFunction
{
    public float x0 = 1.0f;
    public float y0 = 1.0f;
    public float r = 1.0f;

    public override Vector3? EvalFunc(float t)
    {
        if (t >= -Math.PI && t <= Math.PI)
        {
            return new Vector3(r * Mathf.Cos(t) + x0, r * Mathf.Sin(t) + y0, 0);
        }
        else
            return null;

    }

    public override string DescriptionLatex()
    {
        return $"(x - {x0})^2 + (y - {y0})^2 = {r}^2";
    }


    public override string Description()
    {
        return $"(x - {x0})^2 + (y - {y0})^2 = {r}^2";
    }
}

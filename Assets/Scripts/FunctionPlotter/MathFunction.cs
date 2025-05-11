using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class MathFunction: MonoBehaviour
{
    // Start is called before the first frame update

    
    public abstract Vector3 ?EvalFunc(float t);

    public abstract string DescriptionLatex();

    public abstract string Description();

    public void ShowDescription()
    {
        Debug.Log("Function: " + DescriptionLatex());
    }

}

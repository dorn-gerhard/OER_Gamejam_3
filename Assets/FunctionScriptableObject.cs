using UnityEngine;
using static Function;

public enum FunctionType
{
    linear,
    quadratic,
    sin
}

[CreateAssetMenu(fileName = "Function_", menuName = "Functions/FunctionScriptableObject", order = 1)]
public class FunctionScriptableObject : ScriptableObject
{
    public FunctionType type;
    public Sprite sprite;
    public Gradient gradient;

    public float defaultPar1Value;
    public float defaultPar2Value;

    public Vector2 par1Range;
    public Vector2 par2Range;

    public LineLengthData lineLengthData;
}
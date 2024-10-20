using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformation : MonoBehaviour
{
    [SerializeField]
    private TransformationType _type = TransformationType.None;
    [SerializeField]
    private Vector3 _translation = Vector3.zero;
    [SerializeField]
    private float _rotationDegrees = 0f;

    public TransformationType Type => _type;
    public Vector3 Translation => _translation;
    public float RotationDegrees => _rotationDegrees;

    public Vector3 Transform(Vector3 startPos)
    {
        Vector3 result = startPos;

        switch(_type)
        {
            case TransformationType.None:
                break;
            case TransformationType.Translation:
                result = startPos + _translation;
                break;
            case TransformationType.Rotation:
                result = Quaternion.Euler(0, 0, _rotationDegrees) * startPos;
                break;
        }

        return result;
    }

    public override string ToString()
    {
        string str = string.Empty;

        switch (_type)
        {
            case TransformationType.Translation:
                str = "Translation: " + new Vector2(Translation.x, Translation.y).ToString();
                break;
            case TransformationType.Rotation:
                str = "Rotation: " + RotationDegrees + " degrees";
                break;
            case TransformationType.None:
                break;
        }

        return str;
    }
}

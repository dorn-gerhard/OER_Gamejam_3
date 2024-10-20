using UnityEngine;
using UnityEngine.Splines;

public class ChoicePoint : MonoBehaviour
{
    public bool blockNextChoice = false;

    public Gate leftGate;
    public Gate rightGate;

    public SplineContainer leftSplineContainer;
    public SplineContainer rightSplineContainer;

    // Method to return the first spline from the specified container
    public Spline GetSpline(bool useLeftSpline)
    {
        // If useLeftSpline is true, return the first spline from leftSplineContainer
        // If false, return the first spline from rightSplineContainer
        return useLeftSpline ? leftSplineContainer.Splines[0] : rightSplineContainer.Splines[0];
    }
}

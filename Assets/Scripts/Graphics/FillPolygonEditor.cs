using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FillPolygon))]
public class FillPolygonEditor : Editor
{
    public void OnSceneGUI()
    {
        // enable update when editing mesh collider in Editor
        FillPolygon myTarget = (FillPolygon)target;
        myTarget.Init();
    }
}
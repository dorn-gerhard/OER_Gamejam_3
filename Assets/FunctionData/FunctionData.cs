using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FunctionData", menuName = "ScriptableObjects", order = 1)]
public class FunctionData : ScriptableObject
{
    [SerializeField][TextArea(3,10)] public string functionAttributes;
    [SerializeField] public Sprite functionGraph;
    [SerializeField] public bool is_correct;

}

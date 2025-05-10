using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactToDecision : MonoBehaviour
{
    [SerializeField] private SpriteRenderer faceExpressions;
    [SerializeField] private Sprite happyFace;
    [SerializeField] private Sprite sadFace;
    [SerializeField] private Sprite waitingFace;
    private void Start()
    {
        faceExpressions.sprite = waitingFace;
    }
    public void getsRejected()
    {
        faceExpressions.sprite = sadFace;
    }
    
    public void getsAccepted()
    {
        faceExpressions.sprite = happyFace;
    }
}

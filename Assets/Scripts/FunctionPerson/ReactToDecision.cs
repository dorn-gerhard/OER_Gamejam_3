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

    public void doReaction(bool wasAccepted)
    {
        if(wasAccepted)
        {
            getsAccepted();
        }
        else
        {
            getsRejected();
        }
    }
    private void getsRejected()
    {
        faceExpressions.sprite = sadFace;
    }
    
    private void getsAccepted()
    {
        faceExpressions.sprite = happyFace;
    }
}

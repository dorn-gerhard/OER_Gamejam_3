using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReactToDecision : MonoBehaviour
{
    [SerializeField] private SpriteRenderer faceExpressions;
    [SerializeField] private Sprite happyFace;
    [SerializeField] private Sprite sadFace;
    [SerializeField] private Sprite waitingFace;

    public UnityEvent onReactionStarted;
    public UnityEvent onReactionFinish;
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
        onReactionStarted.Invoke();
    }

    private float timeSinceStartedMoving = 0;
    private IEnumerator movePerson(int decision)
    {
        while (timeSinceStartedMoving <4)
        {


            yield return new WaitForEndOfFrame();

            if (decision == 0)
            {
                gameObject.transform.position += new Vector3(-2,0,0)*Time.deltaTime;

            }
            else
            {
                gameObject.transform.position += new Vector3(+2,0,0)*Time.deltaTime;
            }


            timeSinceStartedMoving += Time.deltaTime;
        }
        onReactionFinish.Invoke();
        yield return null;
    }
    private void getsRejected()
    {
        faceExpressions.sprite = sadFace;
        StartCoroutine(movePerson(0));
    }
    
    private void getsAccepted()
    {
        faceExpressions.sprite = happyFace;
        StartCoroutine(movePerson(1));
    }
}

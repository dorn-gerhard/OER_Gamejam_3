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
    [SerializeField] private float MovingSpeed = 5;
    [SerializeField] private float bobbingFrequency = 7f;

    public UnityEvent onReactionStarted;
    public UnityEvent onReactionFinish;

    public delegate void ReadyForAnswer();
    public static event ReadyForAnswer onReadyForAnswer;
    
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
        //float bobbingFrequency = 7f;
        while (timeSinceStartedMoving <2.5f)
        {
            yield return new WaitForEndOfFrame();

            if (decision == 0)
            {
                gameObject.transform.position += new Vector3(-MovingSpeed,0.5f*Mathf.Sin(timeSinceStartedMoving * bobbingFrequency),0)*Time.deltaTime;

            }
            else
            {
                gameObject.transform.position += new Vector3(+MovingSpeed,0.5f*Mathf.Sin(timeSinceStartedMoving * bobbingFrequency),0)*Time.deltaTime;
            }
            
            timeSinceStartedMoving += Time.deltaTime;
        }
        onReactionFinish.Invoke();
    }
    private float TimeCounter = 0;
    private IEnumerator movePersonToPointRoutine(Vector3 GoalPosition)
    {
        //float TimeCounter = 0;
        //float bobbingFrequency = 8f;
        while (Mathf.Abs(GoalPosition.x-gameObject.transform.position.x )>1f)
        //while (Vector3.Distance(gameObject.transform.position, GoalPosition) > 1f)

        {
            //Debug.Log("this is the while loop");
            yield return new WaitForEndOfFrame();

            Vector3 DirectionGoal = GoalPosition-gameObject.transform.position;

            Vector3 DirectionGoalNormalized = DirectionGoal.normalized;
            
            gameObject.transform.position += MovingSpeed * new Vector3(DirectionGoalNormalized.x, 0.1f*Mathf.Sin(TimeCounter*bobbingFrequency),0) * Time.deltaTime;

            //gameObject.transform.position += MovingSpeed * new Vector3(direction, 0, 0) * Time.deltaTime;
            TimeCounter += Time.deltaTime;
        }
        onReadyForAnswer.Invoke();
    }
    public void movePersonToPoint(Vector3 GoalPosition) 
    {
        StartCoroutine(movePersonToPointRoutine(GoalPosition));
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

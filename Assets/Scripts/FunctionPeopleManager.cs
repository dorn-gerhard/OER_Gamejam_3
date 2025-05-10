using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(FunctionDataCollection))]
public class FunctionPeopleManager : MonoBehaviour
{
    [SerializeField] GameObject functionPerson;

    private FunctionDataCollection functionDatas;
    private GameObject currentPerson;
    private FunctionAttributes functionAttributes;
    private ReactToDecision personReact;
    private ReactToDecision movePerson;

    [SerializeField] private Transform functionPersonSpawnTransform;
    [SerializeField] private UnityEvent onAnswerSelected;
    [SerializeField] private UnityEvent onCorrect;
    [SerializeField] private UnityEvent onIncorrect;
    [SerializeField] private GameObject MovingGoalPosition;
    [SerializeField] private UnityEvent onPersonReplaced;
    
    private void Start()
    {
        functionDatas = GetComponent<FunctionDataCollection>();
    }

    public void NewFunctionPerson()
    {
        currentPerson = Instantiate(functionPerson, functionPersonSpawnTransform.position, Quaternion.identity);
        movePerson = currentPerson.GetComponent<ReactToDecision>();
        movePerson.movePersonToPoint(MovingGoalPosition.transform.position);

        functionAttributes = currentPerson.GetComponent<FunctionAttributes>();
        functionAttributes.UpdateFunctionData(functionDatas.nextData());
 
        personReact = currentPerson.GetComponent<ReactToDecision>();
        personReact.onReactionFinish.AddListener(replaceFunctionPerson);
    }

    private void DestroyFunctionPerson()
    {
        functionAttributes = null;
        personReact.onReactionFinish.RemoveAllListeners();
        personReact = null;
        Destroy(currentPerson);
    }
    
    public void Check(bool answer)
    {
        onAnswerSelected.Invoke();
        if (functionAttributes.hasCorrectAttributes() == answer)
        {
            onCorrect.Invoke();
        }
        else
        {
            onIncorrect.Invoke();
        }
        if (personReact != null)
        {
            personReact.doReaction(answer);
        }
    }

    public void replaceFunctionPerson()
    {
        DestroyFunctionPerson();
        NewFunctionPerson();
        onPersonReplaced.Invoke();
    }

    public void WorkdayFinish(Workday.WorkdayReport workdayReport)
    {
        DestroyFunctionPerson();
    }
}

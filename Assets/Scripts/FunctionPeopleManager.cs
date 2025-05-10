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
    
    [SerializeField] private UnityEvent onCorrect;
    [SerializeField] private UnityEvent onIncorrect;

    private void Start()
    {
        functionDatas = GetComponent<FunctionDataCollection>();
        NewFunctionPerson();
    }

    void NewFunctionPerson()
    {
        currentPerson = Instantiate(functionPerson);
        functionAttributes = currentPerson.GetComponent<FunctionAttributes>();
        functionAttributes.UpdateFunctionData(functionDatas.nextData());
        personReact = currentPerson.GetComponent<ReactToDecision>();
    }

    void DestroyFunctionPerson()
    {
        functionAttributes = null;
        personReact = null;
        Destroy(currentPerson);
    }
    
    public void Check(bool answer)
    {
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
        DestroyFunctionPerson();
        NewFunctionPerson();
    }


}

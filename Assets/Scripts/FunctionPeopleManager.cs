using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FunctionPeopleManager : MonoBehaviour
{
    [SerializeField] GameObject functionPerson;
    private FunctionAttributes functionAttributes;
    private ReactToDecision personReact;
    
    [SerializeField] private UnityEvent onCorrect;
    [SerializeField] private UnityEvent onIncorrect;

    private void Start()
    {
        NewFunctionPerson();
    }

    void NewFunctionPerson()
    {
        GameObject person = Instantiate(functionPerson);
        functionAttributes = person.GetComponent<FunctionAttributes>();
        personReact = person.GetComponent<ReactToDecision>();
    }
    
    public void Check(bool answer)
    {
        if (functionAttributes.hasCorrectAttributes == answer)
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

}

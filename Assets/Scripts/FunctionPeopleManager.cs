using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FunctionPeopleManager : MonoBehaviour
{
    [SerializeField] GameObject functionPerson;
    private FunctionAttributes functionAttributes;
    private ReactToDecision personReact;
    
    [SerializeField] private UnityEvent<bool> onAnswered;
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

    }
    
    public void Check(bool answer)
    {
        onAnswered.Invoke(answer);
        if (functionAttributes.hasCorrectAttributes == answer)
        {
            
        }
        else
        {

        }
    }


}

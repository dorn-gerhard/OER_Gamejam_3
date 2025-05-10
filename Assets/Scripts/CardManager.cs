using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CardManager : MonoBehaviour
{
    public string Name = "";
    public Button button;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCardClick()
    {
        Debug.Log("Card click");
    }

    public void OnMyAss(string hat)
    {
        print("myAss");

    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChalkScript : MonoBehaviour
{
    [SerializeField] TMP_Text ReduceNumber;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        print(ReduceNumber.text);
        Board.Instance.TableCard.Reduce();
    }
}

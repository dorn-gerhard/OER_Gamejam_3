using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChalkScript : MonoBehaviour
{
    [SerializeField] TMP_Text ReduceNumberText;
    [SerializeField] int ReduceNumber;

    // Start is called before the first frame update
    void Start()
    {
        ReduceNumberText.text = ReduceNumber.ToString().Replace("1", "I");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        var tableCard = Board.Instance.TableCard;

        if (!tableCard.CanReduce(ReduceNumber))
        {
            // TODO: trigger "fail" sound effect
            Score.Instance.Punish(1);
        }
        else
            Board.Instance.TableCard.Reduce(ReduceNumber);
    }
}

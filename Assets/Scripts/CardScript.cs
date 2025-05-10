using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    [SerializeField] TMP_Text CardNumerator;
    [SerializeField] TMP_Text CardDenominator;

    public CardScript(int nominator, int denominator)
    {
        Setup(nominator, denominator);
    }

    public int nominator { get; private set; } = 1;
    public int denominator { get; private set; } = 2;
    public ActionType actionType { get; private set; } = ActionType.Add;

    public void Setup(int nominator, int denominator)
    {
        this.nominator = nominator;
        this.denominator = denominator;

        UpdateDisplay();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplay();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCardClicked()
    {
        if (this.gameObject.transform.IsChildOf(Player.Instance.transform))
        {
            Board.Instance.TableCard.Add(this);
            Destroy(this.gameObject);
        }
    }

    public void Add(CardScript other)
    {
        if (other.actionType is ActionType.Add)
        {
            int newDenominator = LeastCommonMultiple(denominator, other.denominator);

            int newNominator = nominator * (newDenominator / denominator) +
                other.nominator * (newDenominator / other.denominator);

            denominator = newDenominator;
            nominator = newNominator;

            UpdateDisplay();
        }
        else
            throw new NotImplementedException();
    }

    private void UpdateDisplay()
    {
        if (CardNumerator != null)
        {
            CardNumerator.text = nominator.ToString();
        }

        if (CardDenominator != null)
        {
            CardDenominator.text = denominator.ToString();
        }
    }

    bool CanReduce()
    {
        var gcd = GreatestCommonDivisor(denominator, nominator);
        return gcd is not 1;
    }

    public void Reduce()
    {
        var gcd = GreatestCommonDivisor(denominator, nominator);

        nominator /= gcd;
        denominator /= gcd;

        UpdateDisplay();

        //hacky score system
        Score.Instance.Add(gcd - 1);
    }

    //ripped from stackoverflow
    static int LeastCommonMultiple(int a, int b)
    {
        return (a / GreatestCommonDivisor(a, b)) * b;
    }

    static int GreatestCommonDivisor(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public enum ActionType
    {
        /// <summary>
        /// +
        /// </summary>
        Add,

        /// <summary>
        /// *
        /// </summary>
        Multiply
    }
}

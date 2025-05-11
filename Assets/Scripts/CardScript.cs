using System;
using TMPro;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    [SerializeField] TMP_Text CardNumerator;
    [SerializeField] TMP_Text CardDenominator;

    int numerator = 1;
    int denominator = 2;
    ActionType actionType = ActionType.Add;

    public void Setup(int numerator, int denominator)
    {
        //make sure all cards are irreducible (e.g. 2/4 -> 1/2)
        var gcd = GreatestCommonDivisor(numerator, denominator);
        numerator /= gcd;
        denominator /= gcd;

        this.numerator = numerator;
        this.denominator = denominator;

        UpdateDisplay();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplay();
    }

    public void OnCardClicked()
    {
        if (this.gameObject.transform.IsChildOf(Player.Instance.transform))
        {
            Board.Instance.TableCard.Add(this);

            //replace card (apply cool visual effect?)
            Destroy(this.gameObject);
            Player.Instance.GenerateNewCard();
        }
    }

    public void Add(CardScript other)
    {
        if (other.actionType is ActionType.Add)
        {
            int newDenominator = LeastCommonMultiple(denominator, other.denominator);

            int newNominator = numerator * (newDenominator / denominator) +
                other.numerator * (newDenominator / other.denominator);

            denominator = newDenominator;
            numerator = newNominator;

            UpdateDisplay();
        }
        else
            throw new NotImplementedException();
    }

    private void UpdateDisplay()
    {
        if (CardNumerator == null)
            return;

        CardNumerator.text = numerator.ToString().Replace("1", "I");
        CardDenominator.text = denominator.ToString().Replace("1", "I");

    }

    public bool CanReduce(int userDivisor)
    {
        var gcd = GreatestCommonDivisor(denominator, numerator);
        return gcd % userDivisor is 0;
    }

    public void Reduce(int userDivisor)
    {
        numerator /= userDivisor;
        denominator /= userDivisor;

        UpdateDisplay();

        //hacky score system
        Score.Instance.Add(userDivisor - 1);
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

    internal void Swap()
    {
        (numerator, denominator) = (denominator, numerator);
    }

    internal void Negate()
    {
        numerator = -numerator;
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

using System;
using TMPro;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    [SerializeField] TMP_Text CardNumerator;
    [SerializeField] TMP_Text CardDenominator;
    [SerializeField] GameObject NegativeSign;
    [SerializeField] GameObject FractionPosition;

    int nominator = 1;
    int denominator = 2;
    ActionType actionType = ActionType.Add;

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
        if (CardNumerator == null)
            return;

        CardNumerator.text = nominator.ToString().Replace("1", "I");
        CardDenominator.text = denominator.ToString().Replace("1", "I");

        if (nominator < 0)
        {
            NegativeSign.SetActive(true);
            FractionPosition.transform.position += new Vector3(10, 0, 0);
        }
        else
        {
            NegativeSign.SetActive(false);
            FractionPosition.transform.position += new Vector3(0, 0, 0);
        }
    }

    bool CanReduce()
    {
        var gcd = GreatestCommonDivisor(denominator, nominator);
        return gcd is not 1;
    }

    public void Reduce(int userDivisor)
    {
        var gcd = GreatestCommonDivisor(denominator, nominator);
        if (gcd % userDivisor != 0)
        {
            // TODO: trigger "fail" sound effect
            return;
        }

        nominator /= userDivisor;
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

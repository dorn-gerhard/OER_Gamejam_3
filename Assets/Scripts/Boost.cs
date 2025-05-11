using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Boost : MonoBehaviour
{
    public static Boost Instance;

    // Start is called before the first frame update
    [SerializeField] TMP_Text BoostText;

    int currentBoost = 1;

    void Start()
    {
        Instance = this;
        Update();
    }

    public void Increment(int boost)
    {
        currentBoost += boost;
        Update();
    }

    public void Update()
    {
        BoostText.text = currentBoost.ToString().Replace("1", "I") + "x";
    }
}

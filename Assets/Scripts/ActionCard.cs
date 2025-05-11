using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCard : MonoBehaviour
{
    [SerializeField]
    private GameObject ThisActionCard;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCardClicked()
    {
        ThisActionCard.SetActive(false);
    }
}

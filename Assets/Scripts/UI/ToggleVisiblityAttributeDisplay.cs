using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleVisiblityAttributeDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headingText;
    [SerializeField] private TextMeshProUGUI contentText;
    
    public void ToggleVisibility(bool visible)
    {
        GetComponent<Image>().enabled = visible;
        headingText.enabled = visible;
        contentText.enabled = visible;
    }
}

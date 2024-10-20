using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField]
    private string message;

    private PlantBase _plantBase;

    private void Awake()
    {
        _plantBase = GetComponent<PlantBase>();
        message = "Happy plant :D\n";

        foreach(var effect in _plantBase.DesiredEffects)
        {
            switch(effect)
            {
                case CellEffect.None:
                    message += "no rain pls^^";
                    break;
                case CellEffect.Rain:
                    message += "i like it wet ;)";
                    break;
            }
        }
    }

    private void OnMouseEnter()
    {
        TooltipManager.instance.SetAndShowTooltip(message);
    }

    private void OnMouseExit()
    {
        TooltipManager.instance.HideTooltip();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ConfirmInstructionButton : MonoBehaviour
{
    private Button _confirmButton = null;
    private List<Slot> _slots = new List<Slot>();

    private void Awake()
    {
        _confirmButton = GetComponent<Button>();
    }

    private void Start()
    {
        _confirmButton.interactable = false;
    }

    public void AddSlot(Slot slot)
    {
        _slots.Add(slot);
    }

    public void OnSlotItemChanged()
    {
        foreach(Slot slot in _slots)
        {
            if (!slot.IsOccupied)
            {
                _confirmButton.interactable = false;
                return;
            }
        }

        _confirmButton.interactable = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private ConfirmInstructionButton _confirmBtn = null;

    private DragDrop occupyingItem;

    public DragDrop Item => occupyingItem;

    public bool IsOccupied => occupyingItem != null;

    private void Start()
    {
        _confirmBtn.AddSlot(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            if(eventData.pointerDrag.TryGetComponent(out DragDrop item))
            {
                if(occupyingItem == null)
                {
                    Set(item);
                    item.SetSlot(this);
                    eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
                }
                else
                {
                    eventData.pointerDrag.GetComponent<RectTransform>().position = item.PreDragPosition;
                }
            }
        }
    }

    public void Set(DragDrop item)
    {
        occupyingItem = item;
        _confirmBtn.OnSlotItemChanged();
    }

    public void Free()
    {
        occupyingItem = null;
        _confirmBtn.OnSlotItemChanged();
    }

    public Transformation GetTransformation()
    {
        if(!occupyingItem)
            return null;

        if(occupyingItem.TryGetComponent(out Transformation transformation))
            return transformation;

        return null;
    }
}

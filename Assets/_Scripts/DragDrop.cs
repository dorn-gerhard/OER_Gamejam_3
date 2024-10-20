using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Transformation))]
[RequireComponent(typeof(RawImage))]
public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField]
    private Canvas _canvas;

    private RectTransform _rectTf;
    private CanvasGroup _canvasGroup;
    private Transformation _transformation;
    private RawImage _img;
    private TMP_Text _text;

    private Slot _occupiedSlot = null;
    private Slot _lastSlot = null;
    private Vector3 _preDragPosition;
    private Vector3 _initialPosition;
    public Vector3 PreDragPosition => _preDragPosition;
    public Transformation Transformation => _transformation;

    private void Awake()
    {
        _rectTf = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _transformation = GetComponent<Transformation>();
        _img = GetComponent<RawImage>();
        _text = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        if (_occupiedSlot)
        {
            _occupiedSlot.Free();
            _occupiedSlot = null;
        }
    }

    private void Start()
    {
        StartCoroutine(InitPosDelayed());

        switch(_transformation.Type)
        {
            case TransformationType.Translation:
                _img.color = Color.blue;
                break;
            case TransformationType.Rotation:
                _img.color= Color.red;
                break;
            case TransformationType.None:
                break;
        }

        _text.SetText(_transformation.ToString());
    }

    private IEnumerator InitPosDelayed()
    {
        yield return new WaitForSeconds(1f);
        _initialPosition = _rectTf.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;
        _preDragPosition = _rectTf.position;

        _lastSlot = null;

        if (_occupiedSlot)
        {
            _occupiedSlot.Free();
            _lastSlot = _occupiedSlot;
            _occupiedSlot = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTf.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag.TryGetComponent(out DragDrop other) && other != this)
        {
            if(_occupiedSlot != null)
            {
                var mySlotTemp = _occupiedSlot;

                if(other._lastSlot != null)
                {
                    mySlotTemp = _occupiedSlot;
                    _occupiedSlot = other._lastSlot;
                    _occupiedSlot.Set(this);
                }
                else
                {
                    _occupiedSlot = null;
                }

                other._occupiedSlot = mySlotTemp;
                other._occupiedSlot.Set(other);

                var thisNewPosition = other._preDragPosition;
                other._rectTf.position = _rectTf.position;
                _rectTf.position = thisNewPosition;
            }
            else
            {
                other._rectTf.position = other._preDragPosition;
            }
        }
    }

    public void SetSlot(Slot slot)
    {
        _occupiedSlot = slot;
    }

    public void OnDroppedNowhere()
    {
        if (_occupiedSlot)
        {
            _occupiedSlot.Free();
            _occupiedSlot = null;
        }

        _rectTf.position = _initialPosition;
    }
}

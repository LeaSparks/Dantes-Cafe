using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;
/*
    This is so messy WTF. This is the controller for the card.
    It holds a reference to both the view and the data.
    This is what the player interacts with.
*/
[RequireComponent (typeof(CardDisplay))]
public class IngredientCardController : MonoBehaviour, 
    IPointerExitHandler, IPointerEnterHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private LayerMask _dropTargetMask;

    //Click and Drag
    private bool _IsHeld;
    private Vector3 _dockedLocalPosition;
    private CardDock _currentTarget, _newTarget, _lastDock;
    
    //Hover        
    public event System.Action<CardData> HoverStartEvent;
    public event System.Action HoverEndEvent;

    //Model and View References
    // private IngredientCardData _data;
    // private IngredientCardView _view;
    private CardData _data;
    private CardDisplay _view;

    private CardOutLineVisual outlineVisual;
    public bool IsDraggable = true;
    public bool IsClickable = false;

    public UnityEvent OnClicked;



    private void Awake()
    {
        _view = GetComponent<CardDisplay>();
        outlineVisual = GetComponentInChildren<CardOutLineVisual>(true); 
        _dockedLocalPosition = transform.localPosition;
    }

    private void OnDestroy()
    {
        OnClicked.RemoveAllListeners();
    }

    // -----------------
    // Dragging Card
    // -----------------
#region Card Drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(IsDraggable == false) return;

        _IsHeld = true;
        outlineVisual?.Hide();
        HoverEndEvent?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(IsDraggable == false) return;

        //gameObject.transform.position += (Vector3)eventData.delta;
        Ray ray = GameplayManager.Instance.Camera.ScreenPointToRay(eventData.position);

        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float dist))
        {
            transform.position = ray.GetPoint(dist);
        }

        CheckCardTargeting(eventData); 
    }

    public void OnEndDrag(PointerEventData eventData)
    {          
        if(IsDraggable == false) return;

        _IsHeld = false;
        if (_currentTarget != null && _currentTarget.IsTargetable())
        {
            _currentTarget.OnDrop(this, eventData.position);
            _currentTarget?.OnEndHoveringOver();
        }
        else
        {
            gameObject.transform.DOLocalMove(_dockedLocalPosition, 0.5f);
        }
    }

    private void CheckCardTargeting(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if ((1 <<result.gameObject.layer & _dropTargetMask.value) != 0)
            {
                _newTarget = result.gameObject.GetComponentInParent<CardDock>();
                if (_newTarget != null && _newTarget != _currentTarget)     //If this is a new drop target, change the _current target
                {
                    _currentTarget?.OnEndHoveringOver();
                    _currentTarget = _newTarget;
                    _currentTarget.OnStartHoveringOver(this);
                }
                return;
            }

        }
        _currentTarget?.OnEndHoveringOver();    //If we are no longer hovering over any target
        _currentTarget = null;

    }
#endregion
    // --------------------
    // Hovering over Card
    // --------------------
#region Hovering Card
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_IsHeld || (IsDraggable == false && IsClickable == false))
            return;
        
        outlineVisual?.ShowHover(); //Outline visual 
        HoverStartEvent?.Invoke(_data);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_IsHeld || (IsDraggable == false && IsClickable == false))
            return;

        //gameObject.transform.DOLocalMove(_dockedLocalPosition, 0.5f);
        outlineVisual?.Hide(); //Outline visual 
        HoverEndEvent?.Invoke();
    }

    public void SetDockedPosition(Vector3 position)
    {
        _dockedLocalPosition = position;
    }
#endregion

#region OnClick
    public void OnPointerClick(PointerEventData eventData)      //This should be in controller
    {
        if(IsClickable)
            OnClicked?.Invoke();
    }
#endregion

#region Getters and Setters
    public CardData GetCardData() => _data;
    public void SetCardData(CardData data)
    {
        _view.ApplyCard(data);
        _data = data;
    }
    public CardDock LastDock => _lastDock;
    public void SetLastDock(CardDock dock) {_lastDock = dock;}
    public void SetCardView(CardDisplay view) {_view = view;}
    public CardData Data => _data;
#endregion

}
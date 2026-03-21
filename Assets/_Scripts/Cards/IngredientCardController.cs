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
[RequireComponent (typeof(IngredientCardView))]
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
    public event System.Action<IngredientCardData> HoverStartEvent;
    public event System.Action HoverEndEvent;

    //Model and View References
    private IngredientCardData _data;
    private IngredientCardView _view;
    private CardOutLineVisual outlineVisual;
    public bool IsDraggable = true;
    public bool IsClickable = false;

    public UnityEvent OnClicked;



    private void Awake()
    {
        _view = GetComponent<IngredientCardView>();
        outlineVisual = GetComponentInChildren<CardOutLineVisual>(true); 
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

        gameObject.transform.position += (Vector3)eventData.delta;
        CheckCardTargeting(eventData); 
    }

    public void OnEndDrag(PointerEventData eventData)
    {  
        if(IsDraggable == false) return;

        _IsHeld = false;
        if (_currentTarget != null && _currentTarget.IsTargetable())
        {
            _currentTarget.OnDrop(this, eventData.position);
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
        if (_IsHeld || IsDraggable == false)
            return;
       // outlineVisual?.ShowHover();//Outline visual 
        HoverStartEvent?.Invoke(_data);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_IsHeld || IsDraggable == false)
            return;

        gameObject.transform.DOLocalMove(_dockedLocalPosition, 0.5f);
      //  outlineVisual?.Hide(); //Outline visual 
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
    public IngredientCardData GetCardData() => _data;
    public void SetCardData(IngredientCardData data)
    {
        _view.SetDisplayInformation(data);
        _data = data;
    }
    public CardDock LastDock => _lastDock;
    public void SetLastDock(CardDock dock) {_lastDock = dock;}
    public void SetCardView(IngredientCardView view) {_view = view;}
    public IngredientCardData Data => _data;
#endregion

}
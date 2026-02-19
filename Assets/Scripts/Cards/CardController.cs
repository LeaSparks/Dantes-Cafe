using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class CardController : MonoBehaviour, 
    IPointerExitHandler, IPointerEnterHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private LayerMask _dropTargetMask;
    [SerializeField] IngredientCard _card;

    private bool _IsHeld;
    private Vector3 _dockedLocalPosition;
    
    private Ray ray;
    private CardDock _currentTarget, _newTarget;
    
    //private Sequence _hoverSequence;         
    public event System.Action<IngredientCard> HoverStartEvent;
    public event System.Action HoverEndEvent;

    
    private void Start()
    {
        _dockedLocalPosition = gameObject.transform.localPosition;
    }

    // -----------------
    // Dragging Card
    // -----------------
#region Card Drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        _IsHeld = true;
        //_hoverSequence?.Pause();
        HoverEndEvent?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        gameObject.transform.position += (Vector3)eventData.delta;
        CheckCardTargeting(); 
    }

    public void OnEndDrag(PointerEventData eventData)
    {        
        _IsHeld = false;
        if (_currentTarget != null && _currentTarget.IsTargetable())
        {
            _currentTarget.OnDrop(_card, eventData.position, ref _dockedLocalPosition);
        }
        else
        {
            gameObject.transform.DOLocalMove(_dockedLocalPosition, 0.5f);
        }
    }

    private void CheckCardTargeting()
    {
        ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, _dropTargetMask))  //Check for potential drop target
        {
            _newTarget = hit.collider.gameObject.GetComponentInParent<CardDock>();

            if (_newTarget != null && _newTarget != _currentTarget)     //If this is a new drop target, change the _current target
            {
                _currentTarget?.OnEndHoveringOver();
                _currentTarget = _newTarget;
                _currentTarget.OnStartHoveringOver(_card);
            }
        }
        else                                                            //If we are no longer hovering over any target
        {
            _currentTarget?.OnEndHoveringOver();
            _currentTarget = null;
        }
    }
#endregion
    // --------------------
    // Hovering over Card
    // --------------------
#region Hovering Card
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_IsHeld)
            return;

        //_hoverSequence.Restart();
        HoverStartEvent?.Invoke(_card);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_IsHeld)
            return;

        gameObject.transform.DOLocalMove(_dockedLocalPosition, 0.5f);
        HoverEndEvent?.Invoke();
    }

    public void SetDockedPosition(Vector3 position)
    {
        _dockedLocalPosition = position;
    }
#endregion
}
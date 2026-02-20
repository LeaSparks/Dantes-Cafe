using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class CardDock : MonoBehaviour
{
    //Make sure the GameObject that applies this interface is on the "DragTarget" layer. This is just to help with raycasting for the target
    [SerializeField] protected bool _isTargetable;
    protected RectTransform _rectTransform;

    public abstract void OnDrop(CardController droppedCard, Vector3 cursorPosition);
    public abstract void RefreshCardPositions();
    public abstract void RemoveCardFromCollection(CardController card);
    protected abstract void AddCardToCollection(CardController card);
    
    public virtual void OnStartHoveringOver(CardController hoveringCard) {}       //For additional effects if we want them (like highlighting a hand or stack)
    public virtual void OnEndHoveringOver() {}
    
    public void SetTargetable(bool targetable) { _isTargetable = targetable; }
    public bool IsTargetable() { return _isTargetable; }

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}

using UnityEngine;

public abstract class CardDock : MonoBehaviour
{
    //Make sure the GameObject that applies this interface is on the "DragTarget" layer. This is just to help with raycasting for the target
    [SerializeField] protected bool _isTargetable;

    public abstract void OnDrop(IngredientCard droppedCard, Vector3 cursorPosition, ref Vector3 newDockedPos);
    public abstract Vector3 GetDockPosition();
    public abstract void RemoveCardFromCollection(IngredientCard card);
    
    public virtual void OnStartHoveringOver(IngredientCard hoveringCard) {}       //For additional effects if we want them (like highlighting a hand or stack)
    public virtual void OnEndHoveringOver() {}
    
    public void SetTargetable(bool targetable) { _isTargetable = targetable; }
    public bool IsTargetable() { return _isTargetable; }
}

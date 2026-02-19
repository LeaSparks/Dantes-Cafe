using System.Collections.Generic;
using UnityEngine;

public class Stack : CardDock
{
    private Stack<IngredientCard> _cards = new();
    private OrderCard _associatedOrder;

    public override void OnDrop(IngredientCard droppedCard, Vector3 cursorPosition, ref Vector3 newDockedPos)
    {
        droppedCard.GetCurrentDock()?.RemoveCardFromCollection(droppedCard);
        
        if(_cards.Count > 0)
            _cards.Peek().IsInteractable = false;
        
        _cards.Push(droppedCard);
        newDockedPos = GetDockPosition();
        
        droppedCard.SetCurrentDock(this);
    }

    public override Vector3 GetDockPosition()
    {
        throw new System.NotImplementedException();
        //Get the new local position of the card on the stack
    }

    public override void RemoveCardFromCollection(IngredientCard card)
    {
        if(_cards.Peek() != card || _cards.Peek() == null)
        {
            Debug.LogError("Trying to remove a card that is not at the top of the stack!");
        } else
        {
            _cards.Pop();
            if(_cards.Count > 0)
                _cards.Peek().IsInteractable = true;
        }
    }


    //Getters & Setters
    public void SetAssociatedOrder(OrderCard card) {_associatedOrder = card;}

    public Stack<IngredientCard> GetCards() => _cards;
    public OrderCard GetAssociatedOrder() => _associatedOrder;
}

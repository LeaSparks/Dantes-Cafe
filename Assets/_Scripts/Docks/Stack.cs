using System.Collections.Generic;
using UnityEngine;

public class Stack : CardDock
{
    [SerializeField] float _minimumSpacing = 30f;
    private Stack<CardController> _cards = new();
    private OrderCardData _associatedOrder;

    public override void OnDrop(CardController droppedCard, Vector3 cursorPosition)
    {
        droppedCard.LastDock?.RemoveCardFromCollection(droppedCard);
        AddCardToCollection(droppedCard);
    }

    public override void RefreshCardPositions()
    {
        if(_cards.Count <= 0) return;

        float verticalSpacing = Mathf.Max(_minimumSpacing, _rectTransform.rect.height / _cards.Count);

        Vector3 newOrigin = Vector3.zero;
        CardController[] cardArray = _cards.ToArray();
        int j = 0;
        for(int i = _cards.Count - 1; i >= 0; i--)
        {
            newOrigin.y = j*verticalSpacing;
            j++;
            cardArray[i].gameObject.transform.localPosition = newOrigin;
            cardArray[i].SetDockedPosition(newOrigin);
        }
    }
    
    protected override void AddCardToCollection(CardController card)
    {
        if(_cards.Count > 0)
            _cards.Peek().IsInteractable = false;
        
        _cards.Push(card);
        card.SetLastDock(this);
        card.transform.SetParent(transform);
        RefreshCardPositions();
    }

    public override void RemoveCardFromCollection(CardController card)
    {
        if(_cards.Peek() != card || _cards.Peek() == null)
        {
            Debug.LogError("Trying to remove a card that is not at the top of the stack!");
        } else
        {
            _cards.Pop();
            if(_cards.Count > 0)
                _cards.Peek().IsInteractable = true;
            card.SetLastDock(null);
        }
        RefreshCardPositions();
    }


    //Getters & Setters
    public void SetAssociatedOrder(OrderCardData card) {_associatedOrder = card;}

    public Stack<CardController> GetCards() => _cards;
    public OrderCardData GetAssociatedOrder() => _associatedOrder;
}

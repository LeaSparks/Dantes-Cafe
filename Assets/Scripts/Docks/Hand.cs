using System.Collections.Generic;
using UnityEngine;

public class Hand : CardDock
{
    private List<IngredientCard> _cards = new();
    [SerializeField] int _handSizeLimit = 4;

    public override void OnDrop(IngredientCard droppedCard, Vector3 cursorPosition, ref Vector3 newDockedPos)
    {
        if(_cards.Count < _handSizeLimit)
        {
            droppedCard.GetCurrentDock()?.RemoveCardFromCollection(droppedCard);

            _cards.Add(droppedCard);
            newDockedPos = GetDockPosition();
        
            droppedCard.SetCurrentDock(this);
        } 
    }
    
    public override Vector3 GetDockPosition()
    {
        throw new System.NotImplementedException();
        //Get the new local position of the card in the hand
    }

    public override void RemoveCardFromCollection(IngredientCard card)
    {
        if(_cards.Contains(card))
            _cards.Remove(card as IngredientCard);
    }


    public List<IngredientCard> GetCards => _cards;
}

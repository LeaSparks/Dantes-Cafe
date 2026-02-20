using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hand : CardDock
{
    [SerializeField] List<IngredientCardData> _startingHand = new();
    [SerializeField] GameObject _ingredientCardPrefab;
    [SerializeField] Transform _cardsParentTransform;       //this has to be above everything for raycasting!
    
    [SerializeField] int _handSizeLimit = 4;
    [SerializeField] float _minimumSpacing = 30f;
    private List<CardController> _cards = new();


    void Start()
    {
        CardController temp;
        foreach(var cardData in _startingHand)      //FOR TESTING PURPOSES
        {
            temp = Instantiate(_ingredientCardPrefab, this.transform).GetOrAddComponent<CardController>();
            temp.SetCardData(cardData);
            AddCardToCollection(temp);
        }
    }


    public override void OnDrop(CardController droppedCard, Vector3 cursorPosition)
    {
        if(_cards.Count < _handSizeLimit)
        {
            droppedCard.LastDock?.RemoveCardFromCollection(droppedCard);

            AddCardToCollection(droppedCard);
        } 
    }
    
    public override void RefreshCardPositions()
    {
        if(_cards.Count <= 0) return;

        float horizontalSpacing = Mathf.Max(_minimumSpacing, _rectTransform.rect.width / _cards.Count);

        Vector3 newOrigin = Vector3.zero;
        for(int i = 0; i < _cards.Count; i++)
        {
            newOrigin.x = i*horizontalSpacing + (horizontalSpacing / 2f);   //because the pivot is on the middle on the bottom
            _cards[i].gameObject.transform.localPosition = newOrigin;
            _cards[i].SetDockedPosition(newOrigin);
        }
    }
    
    protected override void AddCardToCollection(CardController card)
    {
        _cards.Add(card);
        card.SetLastDock(this);
        card.transform.SetParent(transform);
        RefreshCardPositions();
    }

    public override void RemoveCardFromCollection(CardController card)
    {
        if(_cards.Contains(card))
            _cards.Remove(card);
        card.SetLastDock(null);
        RefreshCardPositions();
    }


    public List<CardController> GetCards => _cards;
}

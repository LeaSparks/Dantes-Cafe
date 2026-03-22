using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Hand : CardDock
{
    
    [SerializeField] Transform _cardsParentTransform;       //this has to be above everything for raycasting!
    
    [SerializeField] int _handSizeLimit = 4;
    [SerializeField] float _minimumSpacing = 30f;
    private List<IngredientCardController> _cards = new();

    // [Header("Debugging")]
    // [SerializeField] List<IngredientCardData> _startingHand = new();
    // [SerializeField] GameObject _ingredientCardPrefab;
    // public bool EnableDebug;

    public UnityEvent<IngredientCardController> NewIngredientAdded;

    void Start()
    {
        // if(EnableDebug == false) return;

        // IngredientCardController temp;
        // foreach(var cardData in _startingHand)      //FOR TESTING PURPOSES
        // {
        //     temp = Instantiate(_ingredientCardPrefab, this.transform).GetOrAddComponent<IngredientCardController>();
        //     temp.SetCardData(cardData);
        //     AddCardToCollection(temp);
        // }
    }

    public void AddDrawnCardToHand(IngredientCardController card)       //doesnt care about hadn size for now
    {
        AddCardToCollection(card);
    }

#region Dock Controls 
    public override void OnDrop(IngredientCardController droppedCard, Vector3 cursorPosition)
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

        float horizontalSpacing = Mathf.Max(_minimumSpacing, _boxCollider.size.x / _cards.Count);

        Vector3 newOrigin = _boxCollider.bounds.min;
        for(int i = 0; i < _cards.Count; i++)
        {
            newOrigin.x = i*horizontalSpacing + (horizontalSpacing / 2f);   //because the pivot is on the middle on the bottom
            newOrigin.y += 0.05f;
            _cards[i].gameObject.transform.position = newOrigin;
            _cards[i].SetDockedPosition(transform.InverseTransformPoint(newOrigin));
        }

        //spacing for next card:
        horizontalSpacing = Mathf.Max(_minimumSpacing, _boxCollider.size.x / (_cards.Count+1));
        newOrigin.x =  (_cards.Count - 1) * horizontalSpacing + (horizontalSpacing / 2f);
        newOrigin.y += 0.05f;
        
        NextLocalDock = newOrigin;
    }
    
    protected override void AddCardToCollection(IngredientCardController card)
    {
        card.transform.SetParent(transform);
        
        _cards.Add(card);
        card.SetLastDock(this);
        RefreshCardPositions();

        NewIngredientAdded?.Invoke(card);
    }

    public override void RemoveCardFromCollection(IngredientCardController card)
    {
        if(_cards.Contains(card))
            _cards.Remove(card);
        card.SetLastDock(null);
        RefreshCardPositions();
    }
#endregion

    public List<IngredientCardController> GetCards => _cards;
}

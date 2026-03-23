using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stack : CardDock
{
    [SerializeField] float _minimumSpacing = 30f;
    [SerializeField] SpriteRenderer _spriteRenderer;
    private Coroutine _oscillationRoutine;
    private Stack<IngredientCardController> _cards = new();
    private OrderCardData _associatedOrder;
    private List<CardIngredient> _requiredIngredients = new();

    public UnityEvent<Stack, IngredientCardController> NewIngredientAdded;

    void OnDestroy()
    {
        if(_oscillationRoutine != null)
        {
            StopCoroutine(_oscillationRoutine);
            _oscillationRoutine = null;
        }
    }

#region Dock Controls 
    public override void OnDrop(IngredientCardController droppedCard, Vector3 cursorPosition)
    {
        droppedCard.LastDock?.RemoveCardFromCollection(droppedCard);
        AddCardToCollection(droppedCard);
        Debug.Log("yuh");
    }

    public override void RefreshCardPositions()
    {
        if(_cards.Count <= 0) return;

        //actual spacing
        float verticalSpacing = Mathf.Max(_minimumSpacing, _boxCollider.size.y / _cards.Count);

        Vector3 newOrigin = _boxCollider.bounds.min;    //might be getting wring corner here, will have to check
        IngredientCardController[] cardArray = _cards.ToArray();
        int j = 0;
        for(int i = _cards.Count - 1; i >= 0; i--)
        {
            newOrigin.y = j*verticalSpacing;
            newOrigin.z -= 0.05f;
            j++;
            cardArray[i].gameObject.transform.position = newOrigin;
            cardArray[i].SetDockedPosition(transform.InverseTransformPoint(newOrigin));
        }

        Debug.Log($"Set dock to: {newOrigin}, min bounds are: {_boxCollider.bounds.min}");

        //spacing for next card:
        verticalSpacing = Mathf.Max(_minimumSpacing, _boxCollider.size.z / (_cards.Count+1));
        newOrigin.y =  (_cards.Count - 1) * verticalSpacing;
        newOrigin.z -= 0.05f;
        
        NextLocalDock = newOrigin;
    }
    
    protected override void AddCardToCollection(IngredientCardController card)
    {
        if(_cards.Count > 0)
            _cards.Peek().IsDraggable = false;
        
        _cards.Push(card);
        card.SetLastDock(this);
        card.transform.SetParent(transform);
        RefreshCardPositions();

        if(_requiredIngredients.Contains(card.GetCardData().ingredient))
            _requiredIngredients.Remove(card.GetCardData().ingredient);
        
        NewIngredientAdded?.Invoke(this, card);
    }

    public override void RemoveCardFromCollection(IngredientCardController card)
    {
        if(_cards.Peek() != card || _cards.Peek() == null)
        {
            Debug.LogError("Trying to remove a card that is not at the top of the stack!");
        } else
        {
            _cards.Pop();
            if(_cards.Count > 0)
                _cards.Peek().IsDraggable = true;
            card.SetLastDock(null);
        }
        RefreshCardPositions();
        _requiredIngredients.Remove(card.GetCardData().ingredient);
    }
    #endregion



    public override void OnStartHoveringOver(IngredientCardController hoveringCard)
    {
        _oscillationRoutine = StartCoroutine(OscillateBorder());
    }

    public override void OnEndHoveringOver()
    {
        if(_oscillationRoutine != null)
        {
            StopCoroutine(_oscillationRoutine);
            _oscillationRoutine = null;
        }
    }

    private IEnumerator OscillateBorder()
    {
        float time = 0;     //all these bad magic numbers hehehe
        float min = 10;
        float max = 200;
        float frequency = 2f;
        float value = 0;

        Color color = _spriteRenderer.color;
        while(_oscillationRoutine != null && gameObject.activeInHierarchy)
        {
           value = Mathf.Sin(time * frequency * 2f * Mathf.PI);
           color.a = Mathf.Lerp(min, max, value);
           
           yield return null;
           time += Time.deltaTime;
        }
    }

    //Getters & Setters
    public void SetAssociatedOrderCard(OrderCardData card) {
        _associatedOrder = card;
        if(card == null) return;
        _requiredIngredients.Clear();
        _requiredIngredients.AddRange(card.IngredientList);
    }

    public Stack<IngredientCardController> Cards => _cards;
    public List<CardIngredient> RequiredIngredients => _requiredIngredients;
    public OrderCardData GetAssociatedOrder() => _associatedOrder;
}

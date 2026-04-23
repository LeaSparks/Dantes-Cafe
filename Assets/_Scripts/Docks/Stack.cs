using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;

public class Stack : CardDock
{
    [SerializeField] float _minimumSpacing = 0.1f;
    [SerializeField] SpriteRenderer _spriteRenderer;
    private Coroutine _oscillationRoutine;
    [SerializeField] private Stack<IngredientCardController> _cards = new();
    private OrderCardData _associatedOrder;
    private List<CardIngredient> _requiredIngredients = new();
    //private bool _isOrdered = true;
    
    public UnityEvent<Stack, IngredientCardController> NewIngredientAdded;
    public UnityEvent<Stack> IngredientRemoved;
    public Competitor Parent;
    public event Action OnActionTaken;

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

        if(droppedCard.GetCardData().type == CardType.Burning && Parent.UsedBurnCards.Contains(droppedCard.gameObject) == false)
        {
            UseBurnCard(droppedCard.gameObject);    
        }
    }

    public override void RefreshCardPositions()
    {
        if(_cards.Count <= 0) return;

        //actual spacing
        float verticalSpacing =  Mathf.Max(_minimumSpacing, _boxCollider.bounds.size.y / _cards.Count);

        Vector3 newOrigin = transform.InverseTransformPoint(_boxCollider.bounds.min);
        newOrigin.x =  transform.InverseTransformPoint(_boxCollider.bounds.center).x;
        
        IngredientCardController[] cardArray = _cards.ToArray();
        int j = _cards.Count-1;
        for(int i = _cards.Count - 1; i >= 0; i--)
        {
            cardArray[i].gameObject.transform.localRotation = Quaternion.identity;
            
            newOrigin.y = j*verticalSpacing;
            newOrigin.z -= 0.02f;
            j--;
            cardArray[i].gameObject.transform.localPosition = newOrigin;
            cardArray[i].SetDockedPosition(newOrigin);
        }

        //spacing for next card:
        verticalSpacing = _boxCollider.size.z / (_cards.Count+1) ;
        newOrigin.y =  (_cards.Count - 1) * verticalSpacing;
        newOrigin.z -= 0.02f;
        
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
        if (_cards.Peek() != card || _cards.Peek() == null)
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
        
        OnActionTaken?.Invoke();
        IngredientRemoved?.Invoke(this);
    }
    #endregion



    public override void OnStartHoveringOver(IngredientCardController hoveringCard)
    {
        if(_isTargetable)
            _oscillationRoutine = StartCoroutine(OscillateBorder());
    }

    public override void OnEndHoveringOver()
    {
        if(_oscillationRoutine != null)
        {
            StopCoroutine(_oscillationRoutine);
            _oscillationRoutine = null;
             
            var color = _spriteRenderer.color;
            color.a = 0.1f;
            _spriteRenderer.color = color;
        }
    }

    private IEnumerator OscillateBorder()
    {
        float time = 0;     //all these bad magic numbers hehehe
        float min = 0.1f;
        float max = 0.9f;
        float frequency = 0.5f;
        float value = 0;

        Color color = _spriteRenderer.color;
        while(gameObject.activeInHierarchy)
        {
           value = Mathf.Sin(time * frequency * 2f * Mathf.PI);
           color.a = Mathf.Lerp(min, max, value);
           _spriteRenderer.color = color;
           
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

        _requiredIngredients = _requiredIngredients.Except(_cards.Select(c => c.GetCardData().ingredient)).ToList();
        //remove any ingredients already in the stack from the required list
    }

    public bool IsOrdered()
    { int j = 0;
        for(int i = _cards.Count - 1; i >= 0; i--)
        {

            if(i > _associatedOrder.IngredientList.Count - 1) return false;


            if(_cards.ElementAt(i).GetCardData().ingredient != _associatedOrder.IngredientList[j])
                return false;

            j++;
        }
        
        return true;
    }
    public bool WouldBeOrdered(CardData card)
    {
        if(_cards.Count >= _associatedOrder.IngredientList.Count) return false;

        return IsOrdered() && card.ingredient == _associatedOrder.IngredientList[_cards.Count];
    } 

    private void UseBurnCard(GameObject card)
    {
        Parent.UsedBurnCards.Add(card);
        int index = Parent._stacks.IndexOf(this);
        var opposingStack = (Parent is Player) ? 
            GameplayManager.Instance.Enemy.GetStackAtIndex(index) :
            GameplayManager.Instance.Player.GetStackAtIndex(index);
        
        opposingStack.AnimateSingleDiscard(0.5f);
        AudioManager.Instance.PlaySFX(GameplayManager.Instance.FireSFX);
    }

    public void AnimateSingleDiscard(float animTime)
    {
        if(_cards.Count <= 0) return;
        var card = _cards.Peek();
        card.IsClickable = false;
        card.IsDraggable = false;
        card.OutlineVisual.Hide();

        if(card.GetCardData().type == CardType.Burning)
            Parent.UsedBurnCards.Remove(card.gameObject);

        CardManager.Instance.AnimateMoveCardToDock(card.gameObject, GameplayManager.Instance.DiscardPile, null, animTime);
    }

    public Stack<IngredientCardController> Cards => _cards;
    public List<CardIngredient> RequiredIngredients => _requiredIngredients;
    public OrderCardData GetAssociatedOrder() => _associatedOrder;
}

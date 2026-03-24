using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public abstract class Competitor : MonoBehaviour
{
    public static float DISCARD_DELAY = 0.5f;
    protected Hand _hand;
    [SerializeField] protected List<Stack> _stacks;
    [SerializeField] protected TextMeshProUGUI _scoreText;

    protected int _actionsCount;
    protected int _score;
    public Hand Hand => _hand;
    public int Score => _score;

    protected virtual void Start()
    {
        _hand = GetComponentInChildren<Hand>();
        if(_hand == null)
            Debug.Log("NO HAND");
        foreach(var stack in _stacks)
        {
            stack.NewIngredientAdded.AddListener((s, ing) => CheckStackForScore(s));
        }
    }

    public int ActionsCount => _actionsCount;
    public void SetActionsCount(int amount) => _actionsCount = amount;
    public Stack GetStackAtIndex(int index) => _stacks[index];
    private bool _isStackOrdered = false;

    public void CheckStackForScore(Stack stack)
    {
        if(stack.GetAssociatedOrder() == null) return;

        bool isInOrder = true;
        int score = 0;

        var comparisonList = new List<CardIngredient>();
        comparisonList.AddRange(stack.GetAssociatedOrder().IngredientList);
        var stackList = stack.Cards.ToList();

        for(int i = 0; i < stackList.Count && i < stack.GetAssociatedOrder().IngredientList.Count; i++)
        {
            var card = stackList[i].GetCardData();
            
            if (comparisonList.Contains(card.ingredient))
            {

                if(isInOrder && stack.GetAssociatedOrder().IngredientList[i] != stackList[stackList.Count - (i + 1)].GetCardData().ingredient)
                {
                    isInOrder = false;
                    HideAllHighlights(stack);


                } else
                {
                    stackList[i].OutlineVisual.ShowValid();
                }
                
                comparisonList.Remove(card.ingredient);
             
                score += CardDatabase.Instance.GetTypeData(card.type).pointValue;
            } 
            else
            {
                HideAllHighlights(stack);
                return;
            }
        }

         if(stack.Cards.Count != stack.GetAssociatedOrder().IngredientList.Count) return;

        GameplayManager.Instance.RemainingOrders--;

        //update score
        if(isInOrder)
            score += GameplayManager.IN_ORDER_MODIFIER;

        
        AddToScore(score, stack.transform.position);
        GameplayManager.Instance.TurnController.SwapSelfToLast(this);


        //animate cards being discarded
        if(this is Player)
        {
            StartCoroutine(GameplayManager.Instance.Enemy.AnimateDiscard(_stacks.IndexOf(stack)));      //get rid of opposing players stacks too!
        } else
        {
            StartCoroutine(GameplayManager.Instance.Player.AnimateDiscard(_stacks.IndexOf(stack)));
        }

        StartCoroutine(AnimateDiscard(stack));
        GameplayManager.Instance.OrderPanel.RemoveOrderFromSpot(_stacks.IndexOf(stack));

        //check for win
        if(_score > GameplayManager.WINNING_SCORE || GameplayManager.Instance.RemainingOrders <= 0)
        {
            GameplayManager.Instance.GameOver();
        }
        else
        {
            GameplayManager.Instance.AssignOrderToStacks(_stacks.IndexOf(stack));
        }
    }

    public void AddToScore(int score, Vector3 location)
    {
        _score += score;
        
        //animate score popup, 
        EffectsController.Instance.ShowScoreIndicator(score, location);
        
        //update display
        _scoreText.text = $"Score: {_score}";
    }

    public IEnumerator AnimateDiscard(Stack stack)
    {
        yield return new WaitForSeconds(1f);
        Vector3 offset = Vector3.up;
        offset *= this is Player ? -500 : 500; 
        
        foreach(var card in stack.Cards)
        {
            card.transform.DOLocalMove(card.transform.localPosition + offset, DISCARD_DELAY);
            yield return new WaitForSeconds(DISCARD_DELAY);

            GameplayManager.Instance.DiscardIngredient(card.GetCardData());
        }

        while (stack.Cards.Count > 0)
        {
            var c = stack.Cards.Peek();
            stack.RemoveCardFromCollection(c);
            CardManager.Instance.ReturnIngredientCardToPool(c);
        }

    }

    public IEnumerator AnimateDiscard(int stackIndex)
    {
        yield return AnimateDiscard(_stacks[stackIndex]);
    }

    private void HideAllHighlights(Stack stack)
    {
        var stackList = stack.Cards.ToList();
        for(int i = 0; i < stack.Cards.Count; i++)
        {
            stackList[i].OutlineVisual.Hide();
        }
    }
}

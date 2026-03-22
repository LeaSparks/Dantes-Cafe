using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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
        foreach(var stack in _stacks)
        {
            stack.NewIngredientAdded.AddListener((s, ing) => CheckStackForScore(s));
        }
    }

    public int ActionsCount => _actionsCount;
    public void SetActionsCount(int amount) => _actionsCount = amount;
    public Stack GetStackAtIndex(int index) => _stacks[index];

    public void CheckStackForScore(Stack stack)
    {
        if(stack.GetAssociatedOrder() == null) return;

        if(stack.Cards.Count != stack.GetAssociatedOrder().IngredientList.Count) return;

        bool isInOrder = true;
        int score = 0;

        var comparisonList = new List<IngredientCardData>();
        //comparisonList.AddRange(stack.GetAssociatedOrder().IngredientList);
        var stackList = stack.Cards.ToList();

        for(int i = 0; i < stack.Cards.Count; i++)
        {
            var card = stackList[i].GetCardData();
            
            if (comparisonList.Contains(card))
            {
                if(isInOrder && comparisonList[i] != stackList[stackList.Count - (i+1)])
                    isInOrder = false;
                
                comparisonList.Remove(card);
             
                score += card.Value;
            } 
            else
            {
                return;
            }
        }

        GameplayManager.Instance.RemainingOrders--;


        if(isInOrder)
            score += GameplayManager.IN_ORDER_MODIFIER;

        
        AddToScore(score, stack.transform.position);
        //animate cards being discarded
        StartCoroutine(AnimateDiscard(stack));


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
            stack.RemoveCardFromCollection(card);

            GameplayManager.Instance.DiscardIngredient(card.GetCardData());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy : Competitor
{
    private List<Tuple<IngredientCardController, CardDock, float>> _validActions = new(); 

    [Header("Card Selection Constants")]
    [Range(0,1)]
    [SerializeField] private float c_requiredIngredient = 1;
    [Range(0,1)]
    [SerializeField] private float c_ingredientRarity = 1;

    [Header("Action Selection Constants")]
    [Range(0,1)]
    [SerializeField] private float c_orderedCards = 1;
    [Range(0,1)]
    [SerializeField] private float c_requiredForStack = 1;
    [Range(0,1)]
    [SerializeField] private float c_handSizeImportance = 1;
    [SerializeField] private float c_discardBaseScore = 1;     //if card is not in a stack, how likely are you to discard it?        
    

    [Header("Intelligence")]
    [Range(0,1)]
    [SerializeField] private float _choiceThreshold = 0.5f;

    protected override void Start()
    {
        base.Start();
    }

    public IngredientCardController ChooseCard()
    {
        var choices =  new List<Tuple<IngredientCardController, float>>();
        var cards = GameplayManager.Instance.DrawPanel.GetSelectableCards();

        float topScore = 0;
        float score = 0;
        Debug.Log("Scoring cards: " +  cards.Count);
        for (int i = 0; i < cards.Count; i++)
        {            
            score = 0;
             if (IsRequiredIngredient(cards[i].GetCardData()))
                score += c_requiredIngredient;

            score += c_ingredientRarity * CardDatabase.GetDesirabilityOfType(cards[i].GetCardData().type);
            if(score > topScore)
                topScore = score;

            choices.Add(new Tuple<IngredientCardController, float>(cards[i], score));
            Debug.Log("Score: " + score);

        }
        var finalChoices = choices.Where(c => c.Item2 >= topScore * _choiceThreshold).ToList();
        
        int choiceIndex = UnityEngine.Random.Range(0, finalChoices.Count);
        Debug.Log("Chose a card: " + finalChoices[choiceIndex].Item1.name);

        return finalChoices[choiceIndex].Item1;
    }

    public void ChooseActionSequence(int maxActions)
    {
        EvaluateAllActions();
        int maxCount = Math.Min(maxActions, _validActions.Count);
        int actionCount = UnityEngine.Random.Range(0, maxCount+1);

        ChooseNewActionAndAnimate(actionCount);
    }

    public void ChooseNewActionAndAnimate(int counter)
    {
        if(counter <= 0)
        {
            GameplayManager.Instance.ProceedToNextPhase();
            return;
        }

        int actionIndex = UnityEngine.Random.Range(0, _validActions.Count);
        var tuple = _validActions[actionIndex];
        
        Debug.Log($"Chose Action: {tuple.Item1.GetCardData().ingredient} from {tuple.Item1.LastDock} to {tuple.Item2} with score {tuple.Item3}");

        _validActions.Remove(tuple);

        CardManager.Instance.AnimateMoveCardToDock(tuple.Item1.gameObject, tuple.Item2, async () => 
        {
            tuple.Item2.RefreshCardPositions();
            await Awaitable.WaitForSecondsAsync(0.3f);
            ChooseNewActionAndAnimate(counter-1);
        }, 0.7f);
    }

    private bool IsRequiredIngredient(CardData ing)
    {
        foreach(Stack s in _stacks)
        {
            if (s.RequiredIngredients.Contains(ing.ingredient))
            return true;
        }
        return false;
    }

    private void EvaluateAllActions()
    {
        _validActions.Clear();
        float score = 0;
        float topScore = 0;

        //A. Moving from hand
        foreach(var card in _hand.GetCards)
        {
            foreach(var stack in _stacks)
            {
                bool isRequired = stack.RequiredIngredients.Contains(card.GetCardData().ingredient);
                //1. Hand -> Stack
                if (isRequired)
                {
                    score = c_requiredForStack 
                        + (c_orderedCards * (stack.WouldBeOrdered(card.GetCardData()) ? 1 : 0)) 
                        + (c_handSizeImportance * (_hand.GetCards.Count / _hand.HandSizeLimit));
                    
                    _validActions.Add(new Tuple<IngredientCardController, CardDock, float>(card, stack, score));

                    if(score > topScore) topScore = score;
                }
                //2. Hand -> Discard
                score = c_discardBaseScore * (isRequired ? 0 : 1)
                    + (c_handSizeImportance * (_hand.GetCards.Count / _hand.HandSizeLimit));
                    
                _validActions.Add(new Tuple<IngredientCardController, CardDock, float>(card, GameplayManager.Instance.DiscardPile, score));
                if(score > topScore) topScore = score;
            }
        }

        //B. moving from stack
        foreach(var stack in _stacks)
        {
            foreach(var card in stack.Cards)
            {
                bool isRequired = stack.RequiredIngredients.Contains(card.GetCardData().ingredient);
                
                //1. Stack -> Stack
                if (stack.RequiredIngredients.Contains(card.GetCardData().ingredient))
                {
                    score = c_requiredForStack 
                        + (c_orderedCards * (stack.WouldBeOrdered(card.GetCardData()) ? 1 : 0));

                    _validActions.Add(new Tuple<IngredientCardController, CardDock, float>(card, stack, score));
                    if(score > topScore) topScore = score;

                }
                //2. Stack -> Hand
                //TODO: if the card underneath the top card is in order and the card in your hand is next

                //3. Stack -> Discard
                score = c_discardBaseScore * (isRequired ? 0 : 1)
                    + (c_handSizeImportance * (_hand.GetCards.Count / _hand.HandSizeLimit));
                _validActions.Add(new Tuple<IngredientCardController, CardDock, float>(card, GameplayManager.Instance.DiscardPile, score));
                if(score > topScore) topScore = score;

            }
        }

        _validActions = _validActions.Where(c => c.Item3 >= topScore * _choiceThreshold).ToList();
    }

    public void SetAIThreshold(float newThreshold)
    {
        _choiceThreshold = Mathf.Clamp(newThreshold, 0, 1);
    }
}

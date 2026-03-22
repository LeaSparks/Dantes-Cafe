using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Competitor
{
    private List<Tuple<IngredientCardController, CardDock>> _validActions = new(); 

    protected override void Start()
    {
        base.Start();
        foreach(var stack in _stacks)
        {
            stack.NewIngredientAdded.AddListener(ReviewAddedStackCard);
        }
        _hand.NewIngredientAdded.AddListener(ReviewAddedHandCard);
    }

    protected void OnDestroy()
    {
        foreach(var stack in _stacks)
        {
            stack.NewIngredientAdded.RemoveListener(ReviewAddedStackCard);
        } 
        _hand.NewIngredientAdded.RemoveListener(ReviewAddedHandCard);
    }

    public IngredientCardController ChooseCard()
    {
        int i = UnityEngine.Random.Range(0, GameplayManager.Instance.DrawPanel.GetSelectableCards().Count);
        return GameplayManager.Instance.DrawPanel.GetSelectableCards()[i];
    }

    public void ChooseActionSequence(int maxActions)
    {
        int maxCount = Math.Min(maxActions, _validActions.Count);
        int actionCount = UnityEngine.Random.Range(0, maxCount+1);
        // foreach(var a in _validActions)
        //     Debug.Log($"Action: {a.Item1.GetCardData().Name}: {a.Item1.LastDock} -> {a.Item2}"); 

        // Debug.Log($"Action: ---------"); 

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
        Debug.Log($"Chose Action: {tuple.Item1.GetCardData().ingredient} from {tuple.Item1.LastDock} to {tuple.Item2}");

        _validActions.Remove(tuple);

        CardManager.Instance.AnimateMoveCardToDock(tuple.Item1.gameObject, tuple.Item2, async () => 
        {
            tuple.Item2.RefreshCardPositions();
            await Awaitable.WaitForSecondsAsync(0.3f);
            ChooseNewActionAndAnimate(counter-1);
        });
    }

    private void ReviewAddedStackCard(Stack stack, IngredientCardController ing)
    {
        //for now: we can move ingredients from stack to stack that share wanted ingredients
        // foreach(Stack s in _stacks)
        // {
        //     if(s == stack) continue;

        //     if (s.RequiredIngredients.Contains(ing.GetCardData()))
        //     {
        //         _validActions.Add(new Tuple<IngredientCardController, CardDock>(ing, s));
        //     }
        // }

        // REMOVED FOR NOW, INGREDIENTS THAT WERE NOT AT THE TOP OF THE STACK WERE BEING MOVED
    }

    private void ReviewAddedHandCard(IngredientCardController ing)
    {
        foreach(Stack s in _stacks)
        {

            if (s.RequiredIngredients.Contains(ing.GetCardData().ingredient))
            {
                _validActions.Add(new Tuple<IngredientCardController, CardDock>(ing, s));
            }
        }
    }
}

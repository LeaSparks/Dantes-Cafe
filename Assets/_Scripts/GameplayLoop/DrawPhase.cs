using System.Collections.Generic;
using UnityEngine;

public class DrawPhase : IState
{
    //Reference to ingredient card dock
    DrawnCardsPanel drawController = GameplayStateManager.Instance.DrawController;
    
    public void Enter()
    {

        //Draw new ingredient cards from the deck
        List<IngredientCardData> drawnCards = new();
        for (int i = 0; i < drawController.DrawnCardsAmount; i++)
            drawnCards.Add(GameplayStateManager.Instance.DrawNewIngredientCard());

        drawController.UpdateCards(drawnCards, GameplayStateManager.Instance.IsPlayerFirst);
        drawController.UpdateText(GameplayStateManager.Instance.IsPlayerFirst);


        drawController.gameObject.SetActive(true);
    }

    public void Exit()
    {
        drawController.gameObject.SetActive(false);
    }

    public void Update()
    {
        //nothing
    }
}

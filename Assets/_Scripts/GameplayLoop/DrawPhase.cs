using System.Collections.Generic;

public class DrawPhase : IState
{
    //Reference to ingredient card dock    
    public void Enter()
    {
        DrawnCardsPanel drawPanel = GameplayManager.Instance.DrawPanel;
        
        //Draw new ingredient cards from the deck
        List<CardData> drawnCards = new();
        
        for (int i = 0; i < drawPanel.DrawnCardsAmount; i++)
            drawnCards.Add(GameplayManager.Instance.DrawNewIngredientCard());

        drawPanel.SetNewCards(drawnCards, GameplayManager.Instance.TurnController.IsPlayerTurn);

        //GameplayManager.Instance.ProceedToNextPhase();
    }

    public void Exit(){}

    public void Update(){}
}

using System.Collections.Generic;

public class DrawPhase : IState
{
    //Reference to ingredient card dock    
    public void Enter()
    {
        DrawnCardsPanel drawPanel = GameplayManager.Instance.DrawPanel;
        
        //Draw new ingredient cards from the deck
        List<IngredientCardData> drawnCards = new();
        for (int i = 0; i < drawPanel.DrawnCardsAmount; i++)
            drawnCards.Add(GameplayManager.Instance.DrawNewIngredientCard());

        drawPanel.UpdateCards(drawnCards, GameplayManager.Instance.TurnController.IsPlayerTurn);

        drawPanel.gameObject.SetActive(true);
        GameplayManager.Instance.ProceedToNextPhase();
    }

    public void Exit()
    {
        //drawPanel.gameObject.SetActive(false);
    }

    public void Update()
    {
        //nothing
    }
}

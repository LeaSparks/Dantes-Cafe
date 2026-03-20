public class PlayerPhase : IState
{
    private static int ACTIONS_LIMIT = 5;
    private bool _cardChosen;

    public void Enter()
    {
        _cardChosen = false;
        
        GameplayManager.Instance.DrawPanel.gameObject.SetActive(true);
        CardManager.Instance.OnCardReachedTarget.AddListener(ActionPhaseStart);

        GameplayManager.Instance.Player.SetActionsCount(0);
    }

    public void Exit()
    {
        CardManager.Instance.OnCardReachedTarget.RemoveListener(ActionPhaseStart);
    }

    public void Update()
    {
        if(_cardChosen && GameplayManager.Instance.Player.ActionsCount >= ACTIONS_LIMIT)
        {
            GameplayManager.Instance.Player.SetDoneButtonVisibility(false);
            GameplayManager.Instance.ProceedToNextPhase();
        }
    }

    private void ActionPhaseStart()
    {
        _cardChosen = true;
        GameplayManager.Instance.Player.SetDoneButtonVisibility(true);
    }
}

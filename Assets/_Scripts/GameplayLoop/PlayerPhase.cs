using UnityEngine;

public class PlayerPhase : IState
{
    private static int ACTIONS_LIMIT = 5;
    private bool _cardChosen;
    [SerializeField] SoundEffect _onDoneSFX;

    public void Enter()
    {
        _cardChosen = false;
       
        GameplayManager.Instance.DrawPanel.UpdateDrawPanel();
        GameplayManager.Instance.ChangeCameraToView(3);

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
        GameplayManager.Instance.InfoText.text = "Make your actions.";


    }
}

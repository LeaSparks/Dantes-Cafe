using System.Threading.Tasks;
using UnityEngine;

public class EnemyPhase : IState
{

    private static int ACTIONS_LIMIT = 5;
    private static float MAX_CHOOSE_DELAY = 2.5f;
    private static float MIN_CHOOSE_DELAY = 1;

    public void Enter()
    {
        GameplayManager.Instance.DrawPanel.UpdateDrawPanel();
        GameplayManager.Instance.ChangeCameraToView(3);
       

        /*
            Start coroutine:
             - choose random card (with delay)
             - make random actions (with random delay between them)
             - move to next phase
        */
        _ = EnemyTurnRoutine();     //asynch since I cant call coroutine from a non-Monobehaviour class
    }

    public void Exit(){}

    public void Update(){}

    private async Task EnemyTurnRoutine()
    {
        await Awaitable.WaitForSecondsAsync(GetRandomDelay());
        var card = GameplayManager.Instance.Enemy.ChooseCard();
        
        CardManager.Instance.OnCardReachedTarget.AddListener(OnCardReachedHand);

        GameplayManager.Instance.DrawPanel.MoveToHand(card, GameplayManager.Instance.Enemy.Hand, 1f);
        GameplayManager.Instance.ChangeCameraToView(2);
    }

    private float GetRandomDelay()
    {
        return Random.Range(MIN_CHOOSE_DELAY, MAX_CHOOSE_DELAY);
    }
    private async void OnCardReachedHand()
    {
        CardManager.Instance.OnCardReachedTarget.RemoveListener(OnCardReachedHand);
        GameplayManager.Instance.InfoText.text = "Enemy is making actions...";


        await Awaitable.WaitForSecondsAsync(0.2f);
        GameplayManager.Instance.Enemy.ChooseActionSequence(ACTIONS_LIMIT);
    }
}

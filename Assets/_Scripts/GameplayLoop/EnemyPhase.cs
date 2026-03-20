using System.Threading.Tasks;
using UnityEngine;

public class EnemyPhase : IState
{

    private static int ACTIONS_LIMIT = 5;
    private static float MAX_CHOOSE_DELAY = 2.5f;
    private static float MIN_CHOOSE_DELAY = 1;

    public void Enter()
    {
        GameplayManager.Instance.DrawPanel.gameObject.SetActive(true);
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
        GameplayManager.Instance.Enemy.ChooseCard();
        
        int _actionCount  = Random.Range(0, ACTIONS_LIMIT + 1);
        
        for(int i = 0; i < _actionCount; i++)
        {
            await Awaitable.WaitForSecondsAsync(GetRandomDelay());
            GameplayManager.Instance.Enemy.MakeValidAction();
        }

        await Awaitable.WaitForSecondsAsync(1);
        GameplayManager.Instance.ProceedToNextPhase();
    }

    private float GetRandomDelay()
    {
        return Random.Range(MIN_CHOOSE_DELAY, MAX_CHOOSE_DELAY);
    }
}

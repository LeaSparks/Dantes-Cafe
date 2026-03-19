using System.Collections;
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

        //StartCoroutine(EnemyTurnRoutine());     //CANT START A COROUTINE ON A NON_MONOBEHAVIOUR - either move to mono or try async?
    }

    public void Exit()
    {
     
    }

    public void Update()
    {

    }

    private IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitForSeconds(GetRandomDelay());
        GameplayManager.Instance.Enemy.ChooseCard();
        
        int _actionCount  = Random.Range(0, ACTIONS_LIMIT + 1);
        
        for(int i = 0; i < _actionCount; i++)
        {
            yield return new WaitForSeconds(GetRandomDelay());
            GameplayManager.Instance.Enemy.MakeValidAction();
        }

        yield return new WaitForSeconds(0.5f);
    }

    private float GetRandomDelay()
    {
        return Random.Range(MIN_CHOOSE_DELAY, MAX_CHOOSE_DELAY);
    }
}

public class PlayerPhase : IState
{
    private int ACTION_LIMIT;
    private int _actionCount;
    public void Enter()
    {
        _actionCount = 0;
        //wait for player to choose an ingredient card

        //show "Done" button
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }
}

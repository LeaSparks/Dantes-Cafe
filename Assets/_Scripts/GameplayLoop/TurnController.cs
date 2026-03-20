using UnityEngine;

public class TurnController : MonoBehaviour
{
    private Competitor _activeCompetitor;
    public int _turnCounter;

    public bool IsPlayerFirst = true;

    public Competitor ActiveCompetitor => _activeCompetitor;
    public bool IsPlayerTurn => _activeCompetitor is Player;
    public bool IsFirstTurnInRound => _turnCounter == 0;

    void Start()
    {
        _turnCounter = 1;

        if(IsPlayerFirst)
            _activeCompetitor = GameplayManager.Instance.Player;
        else
            _activeCompetitor = GameplayManager.Instance.Enemy;
    }
    public void IncrementTurn()
    {
        _turnCounter = (_turnCounter+1) % 2;
        if(_turnCounter == 0) return;       //because the draft order swaps each time
        _activeCompetitor = IsPlayerTurn ? GameplayManager.Instance.Enemy : GameplayManager.Instance.Player;
    }

}

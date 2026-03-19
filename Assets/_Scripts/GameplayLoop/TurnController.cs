using UnityEngine;

public class TurnController : MonoBehaviour
{
    private Competitor _activeCompetitor;
    private int _turnCounter;
    private Player _player;
    private Enemy _enemy;

    public bool IsPlayerFirst = true;

    public Competitor ActiveCompetitor => _activeCompetitor;
    public bool IsPlayerTurn => _activeCompetitor is Player;
    public bool IsFirstTurnInRound => _turnCounter == 0;

    public void IncrementTurn()
    {
        _turnCounter++;
        _turnCounter = _turnCounter % 2;

        _activeCompetitor = IsPlayerTurn ? _enemy : _player;
    }

    public void SetCompetitors(Player p, Enemy e)
    {
        _player = p;
        _enemy = e;
    }
}

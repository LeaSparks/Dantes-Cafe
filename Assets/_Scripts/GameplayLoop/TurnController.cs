using UnityEngine;

public class TurnController : MonoBehaviour
{
    private Competitor _activeCompetitor;
    public int _turnCounter;

    public bool IsPlayerFirst = true;

    public Competitor ActiveCompetitor => _activeCompetitor;
    public bool IsPlayerTurn => _activeCompetitor is Player;
    public bool IsFirstTurnInRound => _turnCounter == 0;

    private bool stopSwap = false;

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
        if(_turnCounter == 0 && !stopSwap) return;       //because the draft order swaps each time (UNLESS you went last, then scored. then you go last again.)
        _activeCompetitor = IsPlayerTurn ? GameplayManager.Instance.Enemy : GameplayManager.Instance.Player;

        stopSwap = false;
    }

    public void SwapSelfToLast(Competitor competitor)
    {
        if (_turnCounter == 1 && stopSwap == false)  //if you just went last, you go last again (assuming the other player didnt also score)
        {
            stopSwap = true;
            GameplayManager.Instance.InfoText.text = $"{(competitor is Player ? "You" : "Enemy")} will go last again next round.";
        }
    }

}

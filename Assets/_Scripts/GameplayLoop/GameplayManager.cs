using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof( TurnController))]
public class GameplayManager : Singleton<GameplayManager>
{
    public static int IN_ORDER_MODIFIER = 3;
    //public static int WINNING_SCORE = 10;
    public int RemainingOrders;

    private IState _currentState;
    private TurnController _turnController;

    [Header("Competitors")]
    [SerializeField] Player _player;
    [SerializeField] Enemy _enemy;

    //Decks
    [Header("Decks")]
    [SerializeField] private List<CardData> _ingredientsDeck = new();
    private List<CardData> _backupDeck;         //THIS IS FOR TESTING
    private List<OrderCardData> _orderBackup;         //THIS IS FOR TESTING
    [SerializeField] private List<OrderCardData> _orderDeck = new();
    //private List<CardData> _ingredientsDiscard = new();

    [Header("UI Elements")]
    [SerializeField] private DrawnCardsPanel _drawPhasePanel;
    [SerializeField] private OrderPanel _orderPanel;

    [Header("Other stuff that really shouldnt be in here")]
    public Camera Camera;
    [SerializeField] TextMeshProUGUI _infoText;
    [SerializeField] DiscardPile _discardPile;

    [Header("Sounds Effects that also shouldnt be here")]
    public SoundEffect FireSFX;

    private bool _roundOver = false;

    //States
    private DrawPhase _drawPhase = new DrawPhase();
    private PlayerPhase _playerPhase = new PlayerPhase();
    private EnemyPhase _enemyPhase = new EnemyPhase();

    public IState CurrentState => _currentState;

    public DrawnCardsPanel DrawPanel => _drawPhasePanel;
    public OrderPanel OrderPanel => _orderPanel;
    public TurnController TurnController => _turnController;
    public TextMeshProUGUI InfoText => _infoText;
    public DiscardPile DiscardPile => _discardPile;

    public Player Player => _player;
    public Enemy Enemy => _enemy;
    // ----------------------------------------------------
    void Start()
    {
        _turnController = gameObject.GetComponent<TurnController>();

        _backupDeck = new List<CardData>(_ingredientsDeck);
        _orderBackup = new List<OrderCardData>(_orderDeck);
        RemainingOrders = _orderDeck.Count;

        //StartRound();
    }
    public void StartRound()
    {
        _player.SetScore(0);
        _enemy.SetScore(0);
        
        for(int i = 0; i < 3; i++)
            AssignOrderToStacks(i);
            ChangeState(_drawPhase);
    }

    void Update()
    {
        _currentState?.Update();
    }

    public void ResetScene()
    {
        _player.SetScore(0);
        _enemy.SetScore(0);
        
        _player.ClearAllStacks();
        _enemy.ClearAllStacks();

        _ingredientsDeck.Clear();
        _ingredientsDeck = _backupDeck;
        _orderDeck.Clear();
        _orderDeck = _orderBackup;

        _drawPhasePanel.ClearCards();
        _orderPanel.ClearOrderCards();
        _discardPile.ClearPile();

    }

    public void ChangeState(IState state)
    {
        if(_roundOver) return;
        Debug.Log($"Changing to phase: {state}");
        _currentState?.Exit();
        _currentState = state;
        _currentState.Enter();
    }

    public void ProceedToNextPhase()
    {
        if(_turnController.IsFirstTurnInRound || _currentState is DrawPhase)
        {
            _turnController.IncrementTurn();
            if(_turnController.ActiveCompetitor is Player)
            {
                ChangeState(_playerPhase);
            } else
            {
                ChangeState(_enemyPhase);
            } 
        } 
        else
        {
            ChangeState(_drawPhase);
        }
    }
    // ----------------------------------------------------
    #region Deck Controls
    public CardData DrawNewIngredientCard()
    {
        if(_ingredientsDeck.Count > 0)
        {
            int i = Random.Range(0, _ingredientsDeck.Count);
            CardData card = _ingredientsDeck[i];
            _ingredientsDeck.RemoveAt(i);
            
            return card;
        }

        //if there are no more cards in the deck
        if(_discardPile.GetPileCount() == 0)
        {
            Debug.LogWarning("There are no available ingredient cards to draw from! Using random card from backup deck");
            return _backupDeck[Random.Range(0, _backupDeck.Count)];
        }

        //add discarded ingredients back into deck and try again
        _ingredientsDeck.AddRange(_discardPile.Cards);
        StartCoroutine(_discardPile.ClearPile());
        
        return DrawNewIngredientCard();
        
    }

    public void AssignOrderToStacks(int stackIndex)
    {
        if(stackIndex > 2 || stackIndex < 0)
        {
            Debug.LogError("Stack index needs to be btwn 0 and 2");
            stackIndex = 0; //by default for now
        }

        if(_orderDeck.Count == 0)
        {
            _player.GetStackAtIndex(stackIndex).SetAssociatedOrderCard(null);
            _enemy.GetStackAtIndex(stackIndex).SetAssociatedOrderCard(null);
            return;
        }
        
        int i = Random.Range(0, _orderDeck.Count);

        _player.GetStackAtIndex(stackIndex).SetAssociatedOrderCard(_orderDeck[i]);
        _enemy.GetStackAtIndex(stackIndex).SetAssociatedOrderCard(_orderDeck[i]);
        _orderPanel.AssignOrderToSpot(_orderDeck[i], stackIndex);

        _orderDeck.RemoveAt(i);
    }

    #endregion

    public void GameOver(bool playerWon)
    {
        _roundOver = true;
        if(playerWon)
        {
            //PLayer wins, show win screen
            //Debug.Log("CONGRATULATIONS, YOU WON!");
            ProgressionController.Instance.OnRoomComplete();

        } else
        {
            //Enemy wins, show lose screen
            //Debug.Log("YOU LOST!");
            ProgressionController.Instance.OnRoomFailed();
        }
    }

    public void ChangeCameraToView(int viewIndex)
    {
        Camera.GetComponent<MultiStateCameraController>().SwitchState(viewIndex);
    }
}

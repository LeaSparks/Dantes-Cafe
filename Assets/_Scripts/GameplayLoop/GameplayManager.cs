using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof( TurnController))]
public class GameplayManager : Singleton<GameplayManager>
{
    public static int IN_ORDER_MODIFIER = 3;
    public static int WINNING_SCORE = 10;
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
    [SerializeField] private List<OrderCardData> _orderDeck = new();
    private List<CardData> _ingredientsDiscard = new();

    [Header("UI Elements")]
    [SerializeField] private DrawnCardsPanel _drawPhasePanel;
    [SerializeField] private OrderPanel _orderPanel;

    [Header("Other stuff that really shouldnt be in here")]
    public Camera Camera;


    //States
    private DrawPhase _drawPhase = new DrawPhase();
    private PlayerPhase _playerPhase = new PlayerPhase();
    private EnemyPhase _enemyPhase = new EnemyPhase();

    public IState CurrentState => _currentState;

    public DrawnCardsPanel DrawPanel => _drawPhasePanel;
    public OrderPanel OrderPanel => _orderPanel;
    public TurnController TurnController => _turnController;

    public Player Player => _player;
    public Enemy Enemy => _enemy;
    // ----------------------------------------------------
    void Start()
    {
        _turnController = gameObject.GetComponent<TurnController>();
        _drawPhasePanel.gameObject.SetActive(false);

        _backupDeck = new List<CardData>(_ingredientsDeck);
        RemainingOrders = _orderDeck.Count;

        for(int i = 0; i < 3; i++)
            AssignOrderToStacks(i);

        ChangeState(_drawPhase);
    }

    void Update()
    {
        _currentState?.Update();
    }

    public void ChangeState(IState state)
    {
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
        if(_ingredientsDiscard.Count == 0 && _ingredientsDiscard.Count == 0)
        {
            Debug.LogWarning("There are no available ingredient cards to draw from! Using random card from backup deck");
            return _backupDeck[Random.Range(0, _backupDeck.Count)];
        }

        //add discarded ingredients back into deck and try again
        _ingredientsDeck.AddRange(_ingredientsDiscard);
        _ingredientsDiscard.Clear();
        
        return DrawNewIngredientCard();
        
    }

    public void DiscardIngredient(CardData card)
    {
        _ingredientsDiscard.Add(card);
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

    public void GameOver()
    {
        if(Player.Score > Enemy.Score)
        {
            //PLayer wins, show win screen
            Debug.Log("CONGRATULATIONS, YOU WON!");
        } else
        {
            //Enemy wins, show lose screen
            Debug.Log("YOU LOST!");
        }
    }
}

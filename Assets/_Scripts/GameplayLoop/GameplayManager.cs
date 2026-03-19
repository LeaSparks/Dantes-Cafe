using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof( TurnController))]
public class GameplayManager : Singleton<GameplayManager>
{
    public bool GameOver = false;
    private IState _currentState;
    private TurnController _turnController;

    [Header("Competitors")]
    [SerializeField] Player _player;
    [SerializeField] Enemy _enemy;

    //Decks
    [Header("Decks")]
    [SerializeField] private List<IngredientCardData> _ingredientsDeck = new();
    [SerializeField] private List<OrderCardData> _orderDeck = new();
    private List<IngredientCardData> _ingredientsDiscard = new();

    [Header("UI Elements")]
    [SerializeField] private DrawnCardsPanel _drawPhasePanel;


    //States
    private DrawPhase _drawPhase = new DrawPhase();
    private PlayerPhase _playerPhase = new PlayerPhase();
    private EnemyPhase _enemyPhase = new EnemyPhase();

    public IState CurrentState => _currentState;

    public DrawnCardsPanel DrawPanel => _drawPhasePanel;
    public TurnController TurnController => _turnController;

    public Player Player => _player;
    public Enemy Enemy => _enemy;
    // ----------------------------------------------------
    void Start()
    {
        _turnController = gameObject.GetComponent<TurnController>();
        _turnController.SetCompetitors(_player, _enemy);
    }


    void Update()
    {
        _currentState?.Update();
    }

    public void ChangeState(IState state)
    {
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
    public IngredientCardData DrawNewIngredientCard()
    {
        if(_ingredientsDeck.Count > 0)
        {
            int i = Random.Range(0, _ingredientsDeck.Count);
            IngredientCardData card = _ingredientsDeck[i];
            _ingredientsDeck.RemoveAt(i);
            
            return card;
        }

        //if there are no more cards in the deck
        if(_ingredientsDiscard.Count == 0)
        {
            Debug.LogError("There are no available ingredient cards to draw from!");
            return null;
        }

        //add discarded ingredients back into deck and try again
        _ingredientsDeck.AddRange(_ingredientsDiscard);
        _ingredientsDiscard.Clear();
        
        return DrawNewIngredientCard();
        
    }

    public void DiscardIngredient(IngredientCardData card)
    {
        _ingredientsDiscard.Add(card);
    }

    #endregion

}

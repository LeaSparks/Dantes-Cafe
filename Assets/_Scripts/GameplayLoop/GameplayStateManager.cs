using System.Collections.Generic;
using UnityEngine;

public class GameplayStateManager : Singleton<GameplayStateManager>
{
    public bool GameOver = false;
    private IState _currentState;

    //Decks
    [Header("Decks")]
    [SerializeField] private List<IngredientCardData> _ingredientsDeck = new();
    [SerializeField] private List<OrderCardData> _orderDeck = new();
    private List<IngredientCardData> _ingredientsDiscard = new();

    [Header("UI Elements")]
    [SerializeField] private DrawnCardsPanel _drawPhaseController;


    //States
    private DrawPhase _drawPhase = new DrawPhase();
    private PlayerPhase _playerPhase = new PlayerPhase();
    private EnemyPhase _enemyPhase = new EnemyPhase();

    public IState CurrentState => _currentState;
    public DrawPhase DrawPhase => _drawPhase;
    public PlayerPhase PlayerPhase => _playerPhase;
    public EnemyPhase EnemyPhase => _enemyPhase;

    public DrawnCardsPanel DrawController => _drawPhaseController;
    public bool IsPlayerFirst = true;

    // ----------------------------------------------------

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

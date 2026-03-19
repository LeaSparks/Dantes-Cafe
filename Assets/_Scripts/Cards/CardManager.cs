using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class CardManager : Singleton<CardManager>
{
    [Header("Card Pooling")]
    [SerializeField] GameObject _ingredientPrefab;
    [SerializeField] int _ingredentPoolAmt = 10;
    ObjectPool _ingredientPool;

    [SerializeField] GameObject _orderPrefab;
    [SerializeField] int _orderPoolAmt = 6;
    ObjectPool _orderPool;

    [Header("Card Animations")]
    [SerializeField] float _movementSpeed;

    public UnityEvent OnCardReachedTarget = new();

    private void Start()
    {
        _ingredientPool = new ObjectPool(_ingredientPrefab, _ingredentPoolAmt, gameObject);
        _orderPool = new ObjectPool(_orderPrefab, _orderPoolAmt, gameObject);
    }

    public GameObject GetPooledIngredient() => _ingredientPool.GetActivePooledObject();
    public GameObject GetPooledOrder() => _orderPool.GetActivePooledObject(); 


    public void AnimateMoveCard(GameObject card, Transform end)
    {
        float duration = (card.transform.position - end.position).magnitude * _movementSpeed;
        card.transform.DOMove(end.position, duration).OnComplete(() => OnCardReachedTarget?.Invoke());
    }

    public void AnimateMoveCardToHand(GameObject card, Hand hand)
    {
        float duration = (card.transform.position - hand.transform.position).magnitude * _movementSpeed;
        card.transform.DOMove(hand.transform.position, duration).OnComplete(() => 
        {
            OnCardReachedTarget?.Invoke();
            hand.AddDrawnCardToHand(card.GetComponent<IngredientCardController>());
        });
    }
}

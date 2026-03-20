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
    [SerializeField] float _timeToMove;
    [SerializeField] GameObject _cardParent;

    public UnityEvent OnCardReachedTarget = new();

    private void Start()
    {
        _ingredientPool = new ObjectPool(_ingredientPrefab, _ingredentPoolAmt, _cardParent);
        _orderPool = new ObjectPool(_orderPrefab, _orderPoolAmt, _cardParent);
    }

    public GameObject GetPooledIngredient() => _ingredientPool.GetActivePooledObject();
    public GameObject GetPooledOrder() => _orderPool.GetActivePooledObject(); 


    public void AnimateMoveCard(GameObject card, Transform end)
    {
        //float duration = (card.transform.position - end.position).magnitude / _movementSpeed;
        card.transform.DOMove(end.position, _timeToMove).OnComplete(() => OnCardReachedTarget?.Invoke());
    }

    public void AnimateMoveCardToHand(GameObject card, Hand hand)
    {
        card.transform.SetParent(hand.transform);
        //float duration =  card.transform.localPosition.magnitude / _movementSpeed;
        
        card.transform.DOLocalMove(Vector3.zero, _timeToMove).OnComplete(() => 
        {
            OnCardReachedTarget?.Invoke();
            hand.AddDrawnCardToHand(card.GetComponent<IngredientCardController>());
        });
    }
}

using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System;

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
    //[SerializeField] float _timeToMove;
    [SerializeField] GameObject _cardParent;

    public UnityEvent OnCardReachedTarget = new();

    private void Start()
    {
        _ingredientPool = new ObjectPool(_ingredientPrefab, _ingredentPoolAmt, _cardParent);
        _orderPool = new ObjectPool(_orderPrefab, _orderPoolAmt, _cardParent);
    }

    public GameObject GetPooledIngredient() => _ingredientPool.GetActivePooledObject();
    public GameObject GetPooledOrder() => _orderPool.GetActivePooledObject(); 

    public void ReturnIngredientCardToPool(IngredientCardController card)
    {
        card.gameObject.SetActive(false);
        card.transform.SetParent(_cardParent.transform);
    }

    public void ReturnOrderCardToPool(OrderView card)
    {
        card.gameObject.SetActive(false);
        card.transform.SetParent(_cardParent.transform);
    }

    public CardDisplay GetPooledIngredientDisplay()
    {
        return GetPooledIngredient().GetComponent<CardDisplay>();
    }

    public void AnimateMoveCardToDock(GameObject card, CardDock dock, Action onCompleteDelegate, float time = 0.5f)
    {
        card.transform.SetParent(dock.transform);   //jsut in case

        card.transform.DOLocalMove(dock.NextLocalDock, time).OnComplete(() => 
        {
            dock.OnDrop(card.GetComponent<IngredientCardController>(), Vector3.zero);
            OnCardReachedTarget?.Invoke(); 
            onCompleteDelegate();
        });
    }

    public void AnimateMoveCardToPosition(GameObject card, Vector3 target, Action onCompleteDelegate, float time = 0.5f)
    {
        card.transform.DOMove(target, time).OnComplete(() =>
        {
            OnCardReachedTarget?.Invoke();
            onCompleteDelegate();
        });
    }
}

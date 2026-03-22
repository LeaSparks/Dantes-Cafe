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

    public void AnimateMoveCardToDock(GameObject card, CardDock dock, Action onCompleteDelegate)
    {
        card.transform.SetParent(dock.transform);   //jsut in case

        card.transform.DOLocalMove(dock.NextLocalDock, _timeToMove).OnComplete(() => 
        {
            dock.OnDrop(card.GetComponent<IngredientCardController>(), Vector3.zero);
            OnCardReachedTarget?.Invoke(); 
            onCompleteDelegate();
        });
    }
}

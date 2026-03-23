using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderPanel : MonoBehaviour
{
    [SerializeField] List<Transform> _cardDocks = new();
    [SerializeField] float _heightOffset = 0.5f;
    [SerializeField] float _timeToMove = 1f;
    OrderView[] _orders = new OrderView[3];
    bool[] _canAnimateNewCard = new bool[3];

    private void Start()
    {
        for(int i = 0; i < _canAnimateNewCard.Length; i++)
        {
            _canAnimateNewCard[i] = true;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void AssignOrderToSpot(OrderCardData data, int index)
    {
        if(index < 0 || index > 2)
        {
            Debug.LogError($"Trying to assign an order card to an invalid index {index}");
            return;
        }

        var orderView = CardManager.Instance.GetPooledOrder().GetComponent<OrderView>();
        orderView.UpdateView(data);
        
        orderView.transform.position = _cardDocks[index].position - (Vector3.up * _heightOffset);
        _orders[index] = orderView;

        StartCoroutine(AnimateNewCard(index, orderView));

    }

    public List<CardIngredient> GetIngredientsAtSpot(int index)
    {
        if (index < 0 || index > 2)
        {
            Debug.LogError($"Trying to access an order card to an invalid index {index}");
            return null;
        }

        return _orders[index].GetData().IngredientList;
    }

    public void RemoveOrderFromSpot(int index)
    {
        if (index < 0 || index > 2)
        {
            Debug.LogError($"Trying to remove an order card to an invalid index {index}");
            return;
        }
        var card = _orders[index].gameObject;

        if (_orders[index] != null && _orders[index] == this)
            _orders[index] = null;

        _canAnimateNewCard[index] = false;
        card.transform.DOMove(card.transform.position - (Vector3.up *_heightOffset), _timeToMove).OnComplete(() =>
        {
            card.gameObject.SetActive(false);
            _canAnimateNewCard[index] = true;
        });
    }

    private IEnumerator AnimateNewCard(int index, OrderView orderView)
    {
        while (_canAnimateNewCard[index] == false)
        {
            yield return null;
        }

        orderView.transform.DOMove(_cardDocks[index].position, _timeToMove);
    }
}

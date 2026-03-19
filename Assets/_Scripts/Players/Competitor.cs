using UnityEngine;

public abstract class Competitor : MonoBehaviour
{
    protected Hand _hand;
    protected int _actionsCount;

    public Hand Hand => _hand;

    protected virtual void Start()
    {
        _hand = GetComponentInChildren<Hand>();
    }

    public int ActionsCount => _actionsCount;
    public void SetActionsCount(int amount) => _actionsCount = amount;
}

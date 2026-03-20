using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Competitor
{
    public void ChooseCard()
    {
        int i = Random.Range(0, GameplayManager.Instance.DrawPanel.GetSelectableCards().Count);
        var card = GameplayManager.Instance.DrawPanel.GetSelectableCards()[i];
        GameplayManager.Instance.DrawPanel.MoveToHand(card, _hand);
    }

    public void MakeValidAction()
    {
        Debug.Log("Taking some action....");
    }

}

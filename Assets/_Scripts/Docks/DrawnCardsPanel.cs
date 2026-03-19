using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

public class DrawnCardsPanel : MonoBehaviour
{
    [SerializeField] List<IngredientCardController> _ingredientCards = new();
    [SerializeField] TextMeshProUGUI _displayText;
    [SerializeField] int _drawnCardsAmount = 4;

    public int DrawnCardsAmount => _drawnCardsAmount;

    private void Start()
    {
        foreach (var card in _ingredientCards)
        {
            card.OnClicked.AddListener(() =>  MoveToHand(card, GameplayManager.Instance.Player.Hand));
        }
    }
    private void OnDestroy()
    {
        foreach (var card in _ingredientCards)
        {
            card.OnClicked.RemoveAllListeners();
        }
    }

    public void UpdateText(bool isPlayersTurn)
    {
        if (isPlayersTurn)
            _displayText.text = "Choose an ingredient:";
        else
            _displayText.text = "Enemy is choosing an ingredient...";
    }

    public void UpdateCards(List<IngredientCardData> newCards, bool isPlayersTurn)
    {
        if(newCards.Count < _drawnCardsAmount)
        {
            Debug.LogError("There are not enough cards to update!");
            return;
        }

        for (int i = 0; i < newCards.Count; i++)
        {
            _ingredientCards[i].SetCardData(newCards[i]);
            _ingredientCards[i].IsClickable = isPlayersTurn;
            _ingredientCards[i].gameObject.SetActive(true);
        }

        //Todo: Dotween animation of cards being drawn? (start vs end position?)
    }

    public void MoveToHand(IngredientCardController card, Hand targetHand)
    {
        var newCard = CardManager.Instance.GetPooledIngredient();
        newCard.transform.position = card.transform.position;
        newCard.transform.rotation = card.transform.rotation;
        newCard.transform.localScale = card.transform.localScale;

        gameObject.SetActive(false);

        CardManager.Instance.AnimateMoveCardToHand(newCard, targetHand);
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DrawnCardsPanel : MonoBehaviour
{
    [SerializeField] List<IngredientCardController> _ingredientCards = new();
    [SerializeField] TextMeshProUGUI _displayText;
    //[SerializeField] int _drawnCardsAmount = 4;

    public int DrawnCardsAmount => _ingredientCards.Count;

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

    private void OnEnable()
    {
        if(GameplayManager.Instance == null) return;    //skip first call only

        bool playerTurn = GameplayManager.Instance.TurnController.IsPlayerTurn;
        foreach(var card in _ingredientCards)
            card.IsClickable = playerTurn;
        
        UpdateText(playerTurn);
    }

    public void UpdateText(bool isPlayersTurn)
    {
        if (isPlayersTurn)
            _displayText.text = "Choose an ingredient:";
        else
            _displayText.text = "Enemy is choosing an ingredient...";
    }

    public void SetNewCards(List<IngredientCardData> newCards, bool isPlayersTurn)
    {
        if(newCards.Count < _ingredientCards.Count)
        {
            Debug.LogError("There are not enough cards to update!");
            return;
        }
        for (int i = 0; i < newCards.Count; i++)
        {
            _ingredientCards[i].SetCardData(newCards[i]);
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

        newCard.GetComponent<IngredientCardController>().SetCardData(card.Data);

        card.gameObject.SetActive(false);   //So that the player/enemy cannot select this one until it resets
        gameObject.SetActive(false);

        CardManager.Instance.AnimateMoveCardToDock(newCard, targetHand, null);
    }

    public List<IngredientCardController> GetSelectableCards()
    {
        List<IngredientCardController> cards = new();

        foreach(var card in _ingredientCards)
        {
            if(card.gameObject.activeInHierarchy)
                cards.Add(card);
        }
        return cards;
    }
}

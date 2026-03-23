using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DrawnCardsPanel : MonoBehaviour
{
    [SerializeField] List<IngredientCardController> _ingredientCards = new();
    [SerializeField] TextMeshProUGUI _displayText;
    [SerializeField]  SoundEffect _onDrawSFX;
    [SerializeField]  SoundEffect _onShuffleSFX;
    //[SerializeField] int _drawnCardsAmount = 4;

    public int DrawnCardsAmount => _ingredientCards.Count;

    private void Start()
    {
        if(_displayText != null)
            _displayText.gameObject.SetActive(false);
        
        //gameObject.SetActive(false);
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
        if(GameplayManager.Instance.TurnController == null) return;    //skip first call only

        bool playerTurn = GameplayManager.Instance.TurnController.IsPlayerTurn;
        foreach(var card in _ingredientCards)
            card.IsClickable = playerTurn;
        
        UpdateText(playerTurn);
    }

    private void OnDisable()
    {
        if(_displayText != null)
            _displayText.gameObject.SetActive(false);
    }

    public void UpdateText(bool isPlayersTurn)
    {
        if(_displayText == null) return;

        _displayText.gameObject.SetActive(true);
        if (isPlayersTurn)
            _displayText.text = "Choose an ingredient:";
        else
            _displayText.text = "Enemy is choosing an ingredient...";
    }

    public void SetNewCards(List<CardData> newCards, bool isPlayersTurn)
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
        AudioManager.Instance.PlaySFX(_onShuffleSFX);
    }
    

    public void MoveToHand(IngredientCardController card, Hand targetHand)
    {
        var newCard = CardManager.Instance.GetPooledIngredient();
        newCard.transform.position = card.transform.position;
        newCard.transform.rotation = card.transform.rotation;
        newCard.transform.localScale = card.transform.localScale;

        //var data = newCard.GetComponent<>
        newCard.GetComponent<IngredientCardController>().SetCardData(card.Data);

        card.gameObject.SetActive(false);   //So that the player/enemy cannot select this one until it resets
        gameObject.SetActive(false);
        CardManager.Instance.AnimateMoveCardToDock(newCard, targetHand, null);
        AudioManager.Instance.PlaySFX(_onDrawSFX);
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

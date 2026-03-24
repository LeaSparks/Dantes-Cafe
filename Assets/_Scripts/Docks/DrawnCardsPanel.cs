using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class DrawnCardsPanel : MonoBehaviour
{
    [SerializeField] List<IngredientCardController> _ingredientCards = new();
    [SerializeField] Transform _drawOrigin;
    private List<Vector3> _cardPositions = new();
    //[SerializeField] TextMeshProUGUI _displayText;
    [SerializeField]  SoundEffect _onDrawSFX;
    [SerializeField]  SoundEffect _onShuffleSFX;
    //[SerializeField] int _drawnCardsAmount = 4;

    public int DrawnCardsAmount => _ingredientCards.Count;

    private void Awake()
    {
        foreach (var card in _ingredientCards)
        {
            _cardPositions.Add(card.transform.position);
        }
    }

    private void Start()
    {
        //gameObject.SetActive(false);
        foreach (var card in _ingredientCards)
        {
            card.OnClicked.AddListener(() =>  MoveToHand(card, GameplayManager.Instance.Player.Hand, 1f));
            card.OnClicked.AddListener(() =>  GameplayManager.Instance.ChangeCameraToView(0));
        }
    }
    private void OnDestroy()
    {
        foreach (var card in _ingredientCards)
        {
            card.OnClicked.RemoveAllListeners();
        }
    }

    public void UpdateDrawPanel()
    {
        bool playerTurn = GameplayManager.Instance.TurnController.IsPlayerTurn || (GameplayManager.Instance.TurnController.ActiveCompetitor == null && GameplayManager.Instance.TurnController.IsPlayerFirst);
        foreach (var card in _ingredientCards)
            card.IsClickable = playerTurn;

        UpdateText(playerTurn);
    }

    public void UpdateText(bool isPlayersTurn)
    {
        //if(_displayText == null) return;

        //_displayText.gameObject.SetActive(true);
        if (isPlayersTurn)
            GameplayManager.Instance.InfoText.text = "Choose an ingredient:";
        else
            GameplayManager.Instance.InfoText.text = "Enemy is choosing an ingredient...";
        GameplayManager.Instance.ChangeCameraToView(3);
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
            _ingredientCards[i].transform.position = _drawOrigin.position;
            _ingredientCards[i].gameObject.SetActive(true);
        }

        AudioManager.Instance.PlaySFX(_onShuffleSFX);

        StartCoroutine(AnimateCardDraw());
    }
    

    public void MoveToHand(IngredientCardController card, Hand targetHand, float duration)
    {
        var newCard = CardManager.Instance.GetPooledIngredient();
        newCard.transform.position = card.transform.position;
        newCard.transform.rotation = card.transform.rotation;
        newCard.transform.localScale = card.transform.localScale;

        //var data = newCard.GetComponent<>
        newCard.GetComponent<IngredientCardController>().SetCardData(card.Data);

        card.gameObject.SetActive(false);   //So that the player/enemy cannot select this one until it resets
        
        CardManager.Instance.AnimateMoveCardToDock(newCard, targetHand, null, duration);
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

    public IEnumerator AnimateCardDraw()
    {
        for(int i = 0; i < _ingredientCards.Count; i++)
        {
            _ingredientCards[i].transform.position = _drawOrigin.position;
            Action del = (i < _ingredientCards.Count - 1) ? null : () => GameplayManager.Instance.ProceedToNextPhase();
            
            CardManager.Instance.AnimateMoveCardToPosition(_ingredientCards[i].gameObject, _cardPositions[i], del);
            yield return new WaitForSeconds(0.5f);
        }
    }
}

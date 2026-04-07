using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class DiscardPile : CardDock
{

    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] float _heightOffset;
    private Coroutine _oscillationRoutine;
    private List<IngredientCardController> _cards = new();
    

    public override void OnDrop(IngredientCardController droppedCard, Vector3 cursorPosition)
    {
        droppedCard.LastDock?.RemoveCardFromCollection(droppedCard);
        AddCardToCollection(droppedCard);
        NextLocalDock.y += _heightOffset;
    }
    protected override void AddCardToCollection(IngredientCardController card)
    {
        card.transform.SetParent(transform);
        
        _cards.Add(card);
        card.SetLastDock(this);
    }

    //Dont use these
    public override void RefreshCardPositions(){}
    public override void RemoveCardFromCollection(IngredientCardController card){}

    public override void OnStartHoveringOver(IngredientCardController hoveringCard)
    {
        if(_isTargetable)
            _oscillationRoutine = StartCoroutine(OscillateBorder());
    }

    public override void OnEndHoveringOver()
    {
        if(_oscillationRoutine != null)
        {
            StopCoroutine(_oscillationRoutine);
            _oscillationRoutine = null;
             
            var color = _spriteRenderer.color;
            color.a = 0.1f;
            _spriteRenderer.color = color;
        }
    }

    private IEnumerator OscillateBorder()
    {
        float time = 0;     //all these bad magic numbers hehehe
        float min = 0.1f;
        float max = 0.9f;
        float frequency = 0.5f;
        float value = 0;

        Color color = _spriteRenderer.color;
        while(gameObject.activeInHierarchy)
        {
           value = Mathf.Sin(time * frequency * 2f * Mathf.PI);
           color.a = Mathf.Lerp(min, max, value);
           _spriteRenderer.color = color;
           
           yield return null;
           time += Time.deltaTime;
        }
    }

    public IEnumerator ClearPile()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 offset = Vector3.left * 500;        //magic numbers!!
        float clearDelay = 0.3f;
        
        foreach(var card in _cards)
        {
            card.transform.DOLocalMove(card.transform.localPosition + offset, clearDelay)
                .OnComplete(() => CardManager.Instance.ReturnIngredientCardToPool(card));
            
            yield return new WaitForSeconds(clearDelay);
        }
    }

    public List<CardData> Cards => _cards.Select(c => c.Data).ToList();
    public int GetPileCount() {return _cards.Count;}
}

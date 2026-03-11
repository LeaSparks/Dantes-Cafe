using UnityEngine;
using TMPro;

public class IngredientCardView : CardView
{
    [Header("Ingredient Data")]
    [SerializeField] TextMeshProUGUI _typeText;
    [SerializeField] TextMeshProUGUI _valueText;
    [SerializeField] GameObject _flameIcon;

    //-----------------------------------------------------
    //-----------------------------------------------------
    
    public override void SetDisplayInformation(Card card)
    {
        base.SetDisplayInformation(card);

        if(card is IngredientCardData == false)
        {
            Debug.LogError("Cannot use IngredientCardView and a non-ingredient card.");
        }
        
        IngredientCardData ingredient = card as IngredientCardData;
        _typeText.text = ingredient.Type.ToString();
        _valueText.text = ingredient.Value.ToString();

        _flameIcon.SetActive(ingredient.Type == CardType.Fire);

    }
}
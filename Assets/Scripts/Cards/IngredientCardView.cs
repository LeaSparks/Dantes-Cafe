using UnityEngine;
using TMPro;

public class IngredientCardView : CardView
{
    [Header("Ingredient Data")]
    [SerializeField] TextMeshProUGUI _typeText;
    [SerializeField] TextMeshProUGUI _valueText;

    //-----------------------------------------------------
    //-----------------------------------------------------

    public override void SetDisplayInformation(IngredientCard card)
    {
        base.SetDisplayInformation(card);

        if(card is IngredientCard == false)
        {
            Debug.LogError("Cannot use IngredientCardView and a non-ingredient card.");
        }
        
        IngredientCard ingredient = card as IngredientCard;
        _typeText.text = ingredient.Type.ToString();
        _valueText.text = ingredient.Value.ToString();
    }
}
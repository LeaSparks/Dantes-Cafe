using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [Header("References")]
    public Image typeImage;
    public Image ingredientIcon;
    public Image panelImage;

    public TextMeshProUGUI typeText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI valueTextA;
    public TextMeshProUGUI valueTextB;

    public Renderer cardRenderer;

    [Header("Data")]
    public CardDatabase database;

    public void ApplyCard(CardData card)
    {
        // --- TYPE (formerly rarity) ---
        var typeData = database.GetTypeData(card.type);

        typeImage.sprite = typeData.typeSprite;
        typeText.text = typeData.typeLabel;

        if (cardRenderer != null && typeData.typeMaterial != null)
            cardRenderer.material = typeData.typeMaterial;

        if (typeData.isBurning)
        {
            Debug.Log("Special Card!");
        }

        // --- INGREDIENT (formerly type) ---
        var ingredientData = database.GetIngredientData(card.ingredient);

        ingredientIcon.sprite = ingredientData.icon;
        panelImage.color = ingredientData.panelColor;

        titleText.text = ingredientData.title;
        descriptionText.text = ingredientData.description;

        valueTextA.text = ingredientData.valueTextA;
        valueTextB.text = ingredientData.valueTextB;
    }
}
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteAlways]
public class PoppupDisplay : MonoBehaviour
{
    [Header("Sprite Renderers")]
    public Image clipRenderer;   
    public Image pointerRenderer;   
    public Image iconRenderer;   

    [Header("UI Text (World Space Canvas)")]
    public TextMeshProUGUI ingredientNameText;

    [Header("Data")]
    public CardDatabase database;

    [Header("Card")]
    public CardData cardData;

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            ApplyCardSafe();
        }
    }

    void Start()
    {
        ApplyCardSafe();
    }

    public void SetCard(CardData newCard)
    {
        cardData = newCard;
        ApplyCardSafe();
    }

    void ApplyCardSafe()
    {
        if (cardData == null || database == null) return;

        ApplyCard(cardData);
    }

    public void ApplyCard(CardData card)
    {
        var typeData = database.GetTypeData(card.type);

        if (typeData != null)
        {
            if (typeData.typeSprite != null)
                iconRenderer.sprite = typeData.valueSprite;

            pointerRenderer.color = typeData.spriteTint;

        }

        var ingredientData = database.GetIngredientData(card.ingredient);

        if (ingredientData != null)
        {
            ingredientNameText.text = ingredientData.title;
            clipRenderer.color = ingredientData.panelColor;

        }
    }
}
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

    [Header("Gradient Settings")]
    [Range(0f, 1f)]
    public float gradientPosition = 0.5f; // Where we sample the gradient

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
            // Set icon sprite
            if (typeData.valueSprite != null)
                iconRenderer.sprite = typeData.valueSprite;

            // Apply gradient (converted to color)
            if (typeData.spriteGradient != null)
            {
                Color gradientColor = typeData.spriteGradient.Evaluate(gradientPosition);
                pointerRenderer.color = gradientColor;
            }
        }

        var ingredientData = database.GetIngredientData(card.ingredient);

        if (ingredientData != null)
        {
            ingredientNameText.text = ingredientData.title;
            clipRenderer.color = ingredientData.panelColor;
        }
    }
}
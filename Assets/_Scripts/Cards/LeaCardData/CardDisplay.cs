using UnityEngine;
using TMPro;

[ExecuteAlways]
public class CardDisplay : MonoBehaviour
{
    [Header("Sprite Renderers")]
    public SpriteRenderer borderRenderer;   
    public SpriteRenderer keyArtRenderer;   
    public SpriteRenderer colourPanel;

    [Header("UI Text (World Space Canvas)")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    //[Header("Data")]
    //public CardDatabase database;

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
        if (cardData == null || CardDatabase.Instance == null) return;

        ApplyCard(cardData);
    }

    public void ApplyCard(CardData card)
    {
        // ===== TYPE =====
        var typeData = CardDatabase.Instance.GetTypeData(card.type);

        if (typeData != null)
        {
            if (typeData.typeSprite != null)
                borderRenderer.sprite = typeData.typeSprite;

            keyArtRenderer.color = typeData.spriteTint;

            // if (typeData.isBurning)
            //     Debug.Log("Special Card!");
        }

        // ===== INGREDIENT =====
        var ingredientData = CardDatabase.Instance.GetIngredientData(card.ingredient);

        if (ingredientData != null)
        {
            if (ingredientData.icon != null)
                keyArtRenderer.sprite = ingredientData.icon;

         
            colourPanel.color = ingredientData.panelColor;

            titleText.text = ingredientData.title;
            descriptionText.text = ingredientData.description;
        }
    }
}
using UnityEngine;
using TMPro;
using System.Collections.Generic;

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

    [Header("Card")]
    public CardData cardData;

    [Header("Gradient Settings")]
    [Range(16, 512)]
    public int gradientResolution = 128;

    [Tooltip("True = Vertical (top-bottom), False = Horizontal (left-right)")]
    public bool verticalGradient = true;

    // 🔥 Cache so we don’t regenerate identical gradients constantly
    private static Dictionary<Gradient, Texture2D> gradientCache = new Dictionary<Gradient, Texture2D>();

    void OnValidate()
    {
        if (!Application.isPlaying)
            ApplyCardSafe();
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

            if (typeData.spriteGradient != null && keyArtRenderer != null)
            {
                Texture2D gradientTex = GetOrCreateGradientTexture(typeData.spriteGradient);

                // IMPORTANT: use material instance so you don’t overwrite shared material
                keyArtRenderer.material = new Material(keyArtRenderer.sharedMaterial);

                keyArtRenderer.material.SetTexture("_GradientTex", gradientTex);
                keyArtRenderer.material.SetFloat("_Vertical", verticalGradient ? 1f : 0f);
            }
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

    // ===== GRADIENT TEXTURE GENERATION =====
    Texture2D GetOrCreateGradientTexture(Gradient gradient)
    {
        if (gradientCache.TryGetValue(gradient, out Texture2D cachedTex))
            return cachedTex;

        Texture2D tex = new Texture2D(gradientResolution, 1, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;

        for (int i = 0; i < gradientResolution; i++)
        {
            float t = i / (gradientResolution - 1f);
            Color col = gradient.Evaluate(t);
            tex.SetPixel(i, 0, col);
        }

        tex.Apply();

        gradientCache[gradient] = tex;
        return tex;
    }
}
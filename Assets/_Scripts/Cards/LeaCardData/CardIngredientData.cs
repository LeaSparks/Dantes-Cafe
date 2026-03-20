using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Ingredient Data")]
public class CardIngredientData : ScriptableObject
{
    public CardIngredient ingredient;

    [Header("Visuals")]
    public Sprite icon;
    public Color panelColor;

    [Header("Text")]
    public string title;
    public string description;

    public string valueTextA;
    public string valueTextB;
}
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Type Data")]
public class CardTypeData : ScriptableObject
{
    public CardType type;

    [Header("Visuals")]
    public Sprite typeSprite;
    public Sprite valueSprite;
    public Color spriteTint = Color.white;

    [Header("Gameplay")]
    public int pointValue;

    public bool isBurning;
}
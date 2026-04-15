using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Type Data")]
public class CardTypeData : ScriptableObject
{
    public CardType type;

    [Header("Visuals")]
    public Sprite typeSprite;
    public Sprite valueSprite;
    public Gradient  spriteGradient;

    [Header("Gameplay")]
    public int pointValue;

    public bool isBurning;
    public bool isWild;
    public bool isMagic;
}
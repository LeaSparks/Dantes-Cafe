using UnityEngine;

public enum CardType
{
    Basic, Rotten, Golden, Fire
}

[CreateAssetMenu(fileName = "Ingredient", menuName = "ScriptableObjects/Cards/Ingredient")]
public class IngredientCardData : Card
{
    [SerializeField] CardType type;
    [SerializeField] int value;

    public CardType Type => type;
    public int Value => value;
}
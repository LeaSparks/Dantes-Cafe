using UnityEngine;

public enum IngredientType
{
    Basic, Rotten, Golden, Fire
}

[CreateAssetMenu(fileName = "Ingredient", menuName = "ScriptableObjects/Cards/Ingredient")]
public class IngredientCardData : Card
{
    [SerializeField] IngredientType type;
    [SerializeField] int value;

    public IngredientType Type => type;
    public int Value => value;
}
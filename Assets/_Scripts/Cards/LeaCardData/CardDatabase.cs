using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Cards/Card Database")]
public class CardDatabase : ScriptableObject
{
    public List<CardTypeData> typeData;
    public List<CardIngredientData> ingredientData;

    public CardTypeData GetTypeData(CardType type)
    {
        return typeData.Find(t => t.type == type);
    }

    public CardIngredientData GetIngredientData(CardIngredient ingredient)
    {
        return ingredientData.Find(i => i.ingredient == ingredient);
    }
}
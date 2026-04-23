using UnityEngine;
using System.Collections.Generic;

//[CreateAssetMenu(menuName = "Cards/Card Database")]
public class CardDatabase : Singleton<CardDatabase>
{
    public List<CardTypeData> typeData;
    public List<CardIngredientData> ingredientData;

    public CardTypeData GetTypeData(CardType type)
    {
        var found = typeData.Find(t => t.type == type);
        if(found == null) Debug.Log($"COULD NOT FIND TYPE DATA FOR TYPE {type}!!");
        return found;
    }

    public CardIngredientData GetIngredientData(CardIngredient ingredient)
    {
        return ingredientData.Find(i => i.ingredient == ingredient);
    }

    public static float GetDesirabilityOfType(CardType type)
    {
        switch(type)
        {
            case CardType.Rotten:
                return 0;
            case CardType.Basic:
                return 0.25f;
            case CardType.Silver:
                return 0.5f;
            case CardType.Gold:
                return 0.75f;
            case CardType.Diamond:
                return 1f;
            case CardType.Burning:
                return 0.75f;
            default:
                return 0;
        }
    }

    public static CardType GetUpgradedType(CardData data)
    {
        switch (data.type)
        {
            case CardType.Rotten:
                return CardType.Basic;
            case CardType.Basic:
                return CardType.Silver;
            case CardType.Silver:
                return CardType.Gold;
            case CardType.Gold:
                return CardType.Diamond;
            default:
                return data.type;
        }
    }
}
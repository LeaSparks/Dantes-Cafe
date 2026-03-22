using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Order", menuName = "ScriptableObjects/Cards/Order")]
public class OrderCardData : Card
{
    [SerializeField] List<CardIngredient> ingredientList = new();

    public List<CardIngredient> IngredientList => ingredientList;
}

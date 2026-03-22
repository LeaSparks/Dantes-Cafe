using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Order", menuName = "ScriptableObjects/Cards/Order")]
public class OrderCardData : Card
{
    [SerializeField] List<CardIngredientData> ingredientList = new();

    public List<CardIngredientData> IngredientList => ingredientList;
}

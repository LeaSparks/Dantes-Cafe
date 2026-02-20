using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Order", menuName = "ScriptableObjects/Cards/Order")]
public class OrderCardData : Card
{
    [SerializeField] List<IngredientCardData> ingredientList = new();

    public List<IngredientCardData> IngredientList => ingredientList;
}

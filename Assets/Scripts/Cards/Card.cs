using System.Collections.Generic;
using UnityEngine;

public enum IngredientType
{
    Basic, Rotten, Golden, Fire
}

public abstract class Card : ScriptableObject
{
    [SerializeField] protected string cardName;
    [SerializeField] protected Sprite cardImage;
    public bool IsInteractable = true;
    
    public string Name => cardName;
    public Sprite Sprite => cardImage;            //We may just be able to use the image completely

}

[CreateAssetMenu(fileName = "Ingredient", menuName = "ScriptableObjects/Cards/Ingredient")]
public class IngredientCard : Card
{
    [SerializeField] IngredientType type;
    [SerializeField] int value;
    private CardDock currentDock;

    public IngredientType Type => type;
    public int Value => value;
    public CardDock GetCurrentDock() => currentDock;
    public void SetCurrentDock(CardDock dock) {currentDock = dock;}
}


[CreateAssetMenu(fileName = "Order", menuName = "ScriptableObjects/Cards/Order")]
public class OrderCard : Card
{
    [SerializeField] List<IngredientCard> ingredientList = new();

    public List<IngredientCard> IngredientList => ingredientList;
}

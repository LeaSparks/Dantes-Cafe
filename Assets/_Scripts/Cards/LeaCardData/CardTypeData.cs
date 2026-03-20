using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Type Data")]
public class CardTypeData : ScriptableObject
{
    public CardType type;

    [Header("Visuals")]
    public Sprite typeSprite;
    public Material typeMaterial;

    [Header("Info")]
    public string typeLabel;
    public bool isBurning;
}
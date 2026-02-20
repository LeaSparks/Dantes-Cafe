using UnityEngine;

public abstract class Card : ScriptableObject
{
    [SerializeField] protected string cardName;
    [SerializeField] protected Sprite cardImage;
        
    public string Name => cardName;
    public Sprite Sprite => cardImage;            //We may just be able to use the image completely

}


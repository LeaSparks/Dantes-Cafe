using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{    
    [SerializeField] protected TextMeshProUGUI _nameText;
    [SerializeField] protected Image _image;

    //-----------------------------------------------------
    //-----------------------------------------------------

    public virtual void SetDisplayInformation(Card card)
    {
        _nameText.text = card.Name;
        _image.sprite = card.Sprite;
    }
}
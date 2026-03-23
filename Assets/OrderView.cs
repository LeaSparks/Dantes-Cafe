using TMPro;
using UnityEngine;

public class OrderView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _orderText;
    [SerializeField] SpriteRenderer _spriteRenderer;

    private OrderCardData _orderCardData;
   
    public void UpdateView(OrderCardData data)
    {
        _spriteRenderer.sprite = data.Sprite;
        _orderText.text = data.Name;
    }

    public OrderCardData GetData() => _orderCardData;
}

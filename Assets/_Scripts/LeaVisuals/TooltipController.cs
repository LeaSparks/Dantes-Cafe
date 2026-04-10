using UnityEngine;
using UnityEngine.InputSystem;

public class TooltipController : MonoBehaviour
{
    [Header("Tooltip")]
    [SerializeField] private string tooltipName = "New Tooltip";

    
    [Header("References")]
    [SerializeField] private RectTransform uiElement;
    [SerializeField] private Canvas canvas;

    [Header("Settings")]
    [SerializeField] private Vector2 offset = new Vector2(12f, -12f);

    private RectTransform canvasRect;

    private void Awake()
    {
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            mousePos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out anchoredPos
        );

        uiElement.anchoredPosition = anchoredPos + offset;
    }
}
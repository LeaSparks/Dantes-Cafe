using UnityEngine;
using UnityEngine.InputSystem;

public class TooltipSystem : MonoBehaviour //does not work, i tried
{
    [Header("References")]
    public Camera cam;
    public GameObject tooltipObject;

    [Header("Settings")]
    public Vector2 offset = new Vector2(15f, -15f);
    public LayerMask hoverLayer;

    public static GameObject CurrentHoveredObject { get; private set; }

    private RectTransform tooltipRect;

    void Awake()
    {
        tooltipRect = tooltipObject.GetComponent<RectTransform>();
        tooltipObject.SetActive(false);
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        tooltipRect.position = mousePos + offset;

        Ray ray = cam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, hoverLayer))
        {
            CurrentHoveredObject = hit.collider.gameObject;

            if (!tooltipObject.activeSelf)
                tooltipObject.SetActive(true);

            return;
        }

        CurrentHoveredObject = null;

        if (tooltipObject.activeSelf)
            tooltipObject.SetActive(false);
    }
}
using UnityEngine;

public class CardOutLineVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer outlineRenderer;
    [SerializeField] private Card_Visual_Mode mode;

    [Header("Pulse")]
    [SerializeField] private bool usePulse = true;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float pulseAmount = 0.08f;

    private Vector3 _baseScale;
    private bool _isVisible;

    public bool IsValid;

    private void Awake()
    {
        if (outlineRenderer == null)
            outlineRenderer = GetComponent<SpriteRenderer>();

        _baseScale = transform.localScale;
        Hide();
    }

    private void Update()
    {
        if (!_isVisible || !usePulse)
            return;

        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = _baseScale * pulse;
    }

    public void ShowHover()
    {
        if (mode == null || outlineRenderer == null)
            return;

        _isVisible = true;
        outlineRenderer.color = mode.hoverColor;
        outlineRenderer.enabled = true;
        transform.localScale = _baseScale;
    }

    public void ShowValid()
    {
        if (mode == null || outlineRenderer == null)
            return;

        IsValid = true;

        _isVisible = true;
        outlineRenderer.color = mode.validColor;
        outlineRenderer.enabled = true;
        transform.localScale = _baseScale;
    }

    public void ShowInvalid()
    {
        if (mode == null || outlineRenderer == null)
            return;

        IsValid = false;
        _isVisible = true;
        outlineRenderer.color = mode.invalidColor;
        outlineRenderer.enabled = true;
        transform.localScale = _baseScale;
    }

    public void ShowSelected()
    {
        if (mode == null || outlineRenderer == null)
            return;

        _isVisible = true;
        outlineRenderer.color = mode.selectedColor;
        outlineRenderer.enabled = true;
        transform.localScale = _baseScale;
    }

    public void Hide()
    {
        _isVisible = false;
        IsValid = false;

        if (outlineRenderer != null)
            outlineRenderer.enabled = false;

        transform.localScale = _baseScale;
    }
}

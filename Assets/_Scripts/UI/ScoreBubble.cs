using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreBubble : MonoBehaviour
{
    [Header("Value Settings")]
    public float minValue = -9f;
    public float maxValue = 36f;
    
    [Range(-9f, 36f)]
    [SerializeField] private float currentValue = 0f;

    [Header("UI References")]
    public Image mainImage;              
    public Image crackedOverlay;         
    public TextMeshProUGUI scoreText;    

    [Header("Gradients")]
    public Gradient positiveGradient;    // White → Silver → Gold → Diamond → Rainbow
    public Gradient negativeGradient;    

    [Header("Shimmer Settings")]
    public float shimmerSpeed = 2f;
    public float shimmerIntensity = 0.2f;
    public float shimmerStartValue = 30f; 

    [Header("Text Settings")]
    public Color textColor = Color.white; 

    void OnValidate()
    {
        currentValue = Mathf.Round(currentValue);
    }
    
    void Update()
    {
        UpdateVisuals(currentValue);
    }

    public void SetValue(float value)
    {
        currentValue = Mathf.Clamp(value, minValue, maxValue);
        UpdateVisuals(currentValue);
    }

    void UpdateVisuals(float value)
    {
        if (!mainImage) return;

        // --- UPDATE COLOR ---
        if (value >= 0f)
        {
            float t = Mathf.InverseLerp(0f, maxValue, value);
            Color color = positiveGradient.Evaluate(t);

            if (value >= shimmerStartValue)
            {
                float hueShift = Mathf.Sin(Time.time * shimmerSpeed) * shimmerIntensity;
                Color.RGBToHSV(color, out float h, out float s, out float v);
                h += hueShift;
                if (h > 1f) h -= 1f;
                color = Color.HSVToRGB(h, s, v);
            }

            mainImage.color = color;

            if (crackedOverlay)
                crackedOverlay.gameObject.SetActive(false);
        }
        else
        {
            float t = Mathf.InverseLerp(minValue, 0f, value);
            mainImage.color = negativeGradient.Evaluate(t);

            if (crackedOverlay)
            {
                crackedOverlay.gameObject.SetActive(true);
                Color c = crackedOverlay.color;
                c.a = 1f - t; 
                crackedOverlay.color = c;
            }
        }

        // --- UPDATE TEXT ---
        if (scoreText)
        {
            scoreText.text = Mathf.RoundToInt(value).ToString();
            scoreText.color = textColor;
        }
    }
}
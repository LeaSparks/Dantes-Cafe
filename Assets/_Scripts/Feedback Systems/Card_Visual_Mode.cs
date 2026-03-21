using UnityEngine;

[CreateAssetMenu(fileName = "Card_Visual_Mode", menuName = "Visual_Feedback/Card_Visual_Mode")]
public class Card_Visual_Mode : ScriptableObject
{
    [Header("Outline Colors")]
    public Color hoverColor = new Color(0.35f, 0.75f, 1f, 0.75f);
    public Color validColor = new Color(0.35f, 1f, 0.65f, 0.95f);

    [Header("Extra (optional)")]
    public Color invalidColor = new Color(1f, 0.3f, 0.3f, 0.8f);
    public Color selectedColor = new Color(1f, 0.85f, 0.3f, 1f);
}

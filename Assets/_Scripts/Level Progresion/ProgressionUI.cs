using TMPro;
using UnityEngine;

public class ProgressionUI : Singleton<ProgressionUI>
{
    public RoomProgressionScreen ProgressScreen;
    public PerkPopup PerkPopup;
    public TextMeshProUGUI ScoreText;

    public override void Awake()
    {
        base.Awake();
        ShouldDieOnReload = false;
        DontDestroyOnLoad(this);
        Debug.Log($"[ProgressionUI] ui is up");
    }
}

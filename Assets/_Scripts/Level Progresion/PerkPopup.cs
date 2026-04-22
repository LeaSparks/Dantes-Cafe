using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkPopup : MonoBehaviour
{
    [SerializeField] Button _rarityUpgradeBtn;
    [SerializeField] TextMeshProUGUI _rarityPerkText;
    [SerializeField] Button _multiplierUpgradeBtn;
    [SerializeField] TextMeshProUGUI _multiplierPerkText;

    
    void Awake()
    {
        DontDestroyOnLoad(this);
        gameObject.SetActive(false);
    }

    void Start()
    {
        var progController = ProgressionController.Instance;
        _rarityUpgradeBtn?.onClick.AddListener(progController.UpgradeRarityPerk);
        _rarityUpgradeBtn?.onClick.AddListener(progController.ContinueToNextRoom);
       
        _multiplierUpgradeBtn?.onClick.AddListener(progController.UpgradeMultiplierPerk);
        _multiplierUpgradeBtn?.onClick.AddListener(progController.ContinueToNextRoom);
    }

    void OnDestroy()
    {
        _rarityUpgradeBtn?.onClick.RemoveAllListeners();
        _multiplierUpgradeBtn?.onClick.RemoveAllListeners();
    }
    
    public void UpdateAndOpen(float newRarityChance, float newMultiplierChance)
    {
        var progController = ProgressionController.Instance;
        
        gameObject.SetActive(true);
        _rarityPerkText.text = $"RARITY UPGRADE CHANCE\n{progController.CardUpgradeChance * 100f}% -> {newRarityChance*100f}%";
        _multiplierPerkText.text = $"ORDER SCORE MULTIPLIER\n{progController.ScoreMultiplier}x -> {newMultiplierChance}x";
    }
}

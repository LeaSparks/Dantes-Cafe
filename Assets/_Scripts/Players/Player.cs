using UnityEngine;
using UnityEngine.UI;

public class Player : Competitor
{
    [SerializeField] Button _doneButton;
    [SerializeField] SoundEffect _onDoneSFX;

    public void SetDoneButtonVisibility(bool isVisible)
    {
        _doneButton.gameObject.SetActive(isVisible);
    }

    protected override void Start()
    {
        base.Start();
        _doneButton.onClick.AddListener(OnDoneButtonClick);
        SetDoneButtonVisibility(false);
    }

    void OnDestroy()
    {
        _doneButton.onClick.RemoveListener(OnDoneButtonClick);
    }

    public void OnDoneButtonClick()
    {
        AudioManager.Instance.PlaySFX(_onDoneSFX); 
        SetDoneButtonVisibility(false);
        GameplayManager.Instance.ProceedToNextPhase();
    }
}

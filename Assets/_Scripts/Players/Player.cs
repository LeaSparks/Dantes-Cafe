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
        
        foreach(var stack in _stacks)
            stack.OnActionTaken += ActionTaken;
        _hand.OnActionTaken += ActionTaken;
    }

    void OnDestroy()
    {
        _doneButton.onClick.RemoveListener(OnDoneButtonClick);

        foreach(var stack in _stacks)
            stack.OnActionTaken -= ActionTaken;
        _hand.OnActionTaken -= ActionTaken;
    }

    public void OnDoneButtonClick()
    {
        AudioManager.Instance.PlaySFX(_onDoneSFX); 
        SetDoneButtonVisibility(false);
        GameplayManager.Instance.ProceedToNextPhase();
    }

    public void ActionTaken()
    {
        _actionsCount ++;
        //could do check here but the player phase is alreay checking in update
    }
}

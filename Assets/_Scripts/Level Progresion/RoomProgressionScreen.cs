using System;
using DG.Tweening;
using TMPro;

using UnityEngine;

public class RoomProgressionScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] RectTransform _textPanel;
    [SerializeField] CanvasGroup _textCanvasGroup;
    [SerializeField] TextMeshProUGUI _nextFloorText;
    [SerializeField] TextMeshProUGUI _currentFloorText;
    [SerializeField] TextMeshProUGUI _previousFloorText;

    [Header("Animation")]
    [SerializeField] float _fadeInDuration;
    [SerializeField] float _fadeOutDuration;
    [SerializeField] float _floorTransitionDuration;
    [SerializeField] float _pauseDuration;

    private Vector2 _defaultPosition;
    private Vector2 _heightOffset;

    public event Action OnTransitionComplete;

    public void Start()
    {
        gameObject.SetActive(false);
        _defaultPosition = _textPanel.localPosition;
        _heightOffset = new(0f, _textPanel.sizeDelta.y/3f);     //height
    }

    public void NextFloor(float upperNum, float currentNum)
    {
        Debug.Log($"[RoomProgressScreen] starting nextFloor transition");

        ResetState();
        _currentFloorText.text = currentNum + "F";
        _nextFloorText.text = upperNum + "F";

        Sequence seq = DOTween.Sequence();
        seq.Append(_textCanvasGroup.DOFade(1f, _fadeInDuration));
        seq.AppendInterval(_pauseDuration);
        seq.Append(_textPanel.DOLocalMove(-_heightOffset, _floorTransitionDuration));
        seq.AppendInterval(_pauseDuration);
        seq.Append(_textCanvasGroup.DOFade(0f, _fadeOutDuration));
        seq.OnKill(OnComplete);

    }

    public void PreviousFloor(float lowerNum, float currentNum)
    {
        Debug.Log($"[RoomProgressScreen] starting prevFloor transition");

        ResetState();
        _currentFloorText.text = currentNum + "F";
        _previousFloorText.text = lowerNum + "F";

        gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(_textCanvasGroup.DOFade(1f, _fadeInDuration));
        seq.AppendInterval(_pauseDuration);
        seq.Append(_textPanel.DOLocalMove(_heightOffset, _floorTransitionDuration));
        seq.AppendInterval(_pauseDuration);
        seq.Append(_textCanvasGroup.DOFade(0f, _fadeOutDuration));
        seq.OnKill(OnComplete);

    }

    public void ShowCurrentFloor(float currentNum)
    {
        Debug.Log($"[RoomProgressScreen] starting currentFloor transition");

        ResetState();
        _currentFloorText.text = currentNum + "F";

        gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(_textCanvasGroup.DOFade(1f, _fadeInDuration));
        seq.AppendInterval(_pauseDuration*1.5f);
        seq.Append(_textCanvasGroup.DOFade(0f, _fadeOutDuration));
        seq.OnKill(OnComplete);
    }

    private void ResetState()
    {
        gameObject.SetActive(true);
        _textPanel.localPosition = _defaultPosition;
        //_textPanel.anchoredPosition = _defaultPosition;
        _textCanvasGroup.alpha = 0f;
    }

    private void OnComplete()
    {
        Debug.Log($"[RoomProgressScreen] transition complete");
        //OnTransitionComplete?.Invoke();
        gameObject.SetActive(false);
        GameplayManager.Instance.StartRound();
        
    }

}

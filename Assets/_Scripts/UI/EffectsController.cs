using System;
using UnityEngine;

public class EffectsController : Singleton<EffectsController>
{
    [Header("Score Effects")]
    [SerializeField] TextIndicator _scoreIndicator;
    [SerializeField] SoundEffect   _onScoreSFX;


    public void PlayEffectAtPosition(ParticleSystem effect, Vector3 targetPosition)
    {
        effect.gameObject.transform.position = targetPosition;
        effect.Play();
    }  
    
    public void ShowScoreIndicator(int amount, Vector3 target)
    {
        string text = ((amount >= 0) ? "+" : "-") + Math.Abs(amount).ToString();
        _scoreIndicator.ShowIndicatorAtWorldTarget(text, target);      //change it to the other one when we move away from convas stuff
        AudioManager.Instance.PlaySFX(_onScoreSFX);
    }
}

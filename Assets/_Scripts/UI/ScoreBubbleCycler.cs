using UnityEngine;

public class ScoreBubbleCycler : MonoBehaviour
{
    [Header("Reference")]
    public ScoreBubble scoreBubble;

    [Header("Cycle Settings")]
    public float interval = 0.5f;
    public bool autoCycle = true;

    private float timer;
    private int currentValue;

    void Start()
    {
        if (scoreBubble == null) return;

        // Start at min value
        currentValue = Mathf.RoundToInt(scoreBubble.minValue);
        scoreBubble.SetValue(currentValue);
    }

    void Update()
    {
        if (!autoCycle || scoreBubble == null) return;

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            Increment();
        }
    }

    public void Increment()
    {
        currentValue++;

        if (currentValue > scoreBubble.maxValue)
        {
            currentValue = Mathf.RoundToInt(scoreBubble.minValue);
        }

        scoreBubble.SetValue(currentValue);
    }

    public void Decrement()
    {
        currentValue--;

        if (currentValue < scoreBubble.minValue)
        {
            currentValue = Mathf.RoundToInt(scoreBubble.maxValue);
        }

        scoreBubble.SetValue(currentValue);
    }
}
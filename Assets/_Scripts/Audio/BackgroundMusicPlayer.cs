using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    [SerializeField] private SoundEffect _backgroundMusic;

    private void Start()
    {
        if (AudioManager.Instance != null && _backgroundMusic != null)
        {
            AudioManager.Instance.SetMusic(_backgroundMusic);
        }
    }
}
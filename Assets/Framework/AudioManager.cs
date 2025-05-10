using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _globalSource;
    [SerializeField] private AudioData _audioData;
    
    public static AudioManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        SongController.Instance.OnBeatTrigger += _ => PlaySound("Metronome");
        SongController.Instance.OnCountdownTrigger += _ => PlaySound("Countdown");
    }

    public void PlaySound(string id)
    {
        if (_globalSource != null && _audioData.TryGetValue(id, out AudioClip clip))
        {
            _globalSource.PlayOneShot(clip);
        }
    }
}

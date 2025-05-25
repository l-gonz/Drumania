using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _globalSource;
    [SerializeField] private AudioData _audioData;

    private bool hasPlayedFirstBar;
    private bool isFirstBeat = true;
    
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
        SongController.Instance.OnBeatTrigger += PlayBeatSound;
        SongController.Instance.OnCountdownTrigger += _ => PlaySound(AudioString.Countdown);
    }

    public void PlaySound(AudioString id)
    {
        if (_globalSource != null && _audioData.TryGetValue(id, out AudioClip clip))
        {
            _globalSource.PlayOneShot(clip);
        }
    }

    private void PlayBeatSound(float time, int beatIndex)
    {
        if (beatIndex == 0) hasPlayedFirstBar = !isFirstBeat;
        
        if (!hasPlayedFirstBar)
        {
            PlaySound(AudioString.Drumsticks);
            isFirstBeat = false;
        }
        else
        {
            PlaySound(AudioString.Metronome);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SongController : MonoBehaviour
{
    [SerializeField] private int _countdown = 4;
    [SerializeField] private float _hitThreshold = .1f;
    [SerializeField] private SongData _currentSong;

    public event Action<List<Bar>, int> OnSongStarted;
    public event Action OnSongEnded;
    public event Action<int> OnCountdownTrigger;
    public event Action<float, int> OnBeatTrigger;

    public static SongController Instance { get; private set; }
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

    public float CurrentTime => currentTime;

    private int currentTempo;
    private int currentBarIndex;
    private int currentBeatIndex;
    private float currentTime;

    private float currentBarStartTime;

    private bool hasSongStarted;
    private float lastTriggerTime;

    private float beatDuration => 60f / currentTempo;
    private float currentBarDuration => song[currentBarIndex].BeatsPerBar * beatDuration;
    private bool hasSongEnded => currentBarIndex >= song.Count;

    private Dictionary<Note, bool> scoredBeats = new Dictionary<Note, bool>();
    private List<Bar> song;

    private void Start()
    {
        currentTempo = _currentSong.DefaultTempo;
        currentTime = - _countdown - 0.3f;

        // Add count in and repeat bars
        song = _currentSong.Bars.Prepend(new Bar()
        {
            BeatsPerBar = 4,
            BeatLength = .25f,
            Notes = Array.Empty<Note>(),
        }).SelectMany(bar => Enumerable.Repeat(bar, bar.Repeat)).ToList();

        // Populate score dictionary
        var beatDuration = 60f / currentTempo;
        var accumulatedTime = 0f;
        foreach (var bar in song)
        {
            var barDuration = bar.BeatsPerBar * beatDuration;
            foreach (var beat in bar.Notes)
            {
                scoredBeats.Add(beat with {Time = accumulatedTime + beat.Time * barDuration}, false);
            }
            accumulatedTime += barDuration;
        }
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (!hasSongStarted)
        {
            HandleCountdown();
        }
        
        if (hasSongStarted && !hasSongEnded)
        {
            HandleBeat();
        }
    }

    private void HandleCountdown()
    {
        var nextCount = Mathf.Round(currentTime) + 0.45f;
        if (currentTime >= nextCount && lastTriggerTime != nextCount)
        {
            lastTriggerTime = nextCount;
            Debug.Log($"Countdown {lastTriggerTime}: {currentTime}");
            OnCountdownTrigger?.Invoke((int)lastTriggerTime);
        }

        if (currentTime >= 0)
        {
            hasSongStarted = true;
            lastTriggerTime = -beatDuration;
            Debug.Log("Song started!");
            OnSongStarted?.Invoke(song, currentTempo);
        }
    }

    private void HandleBeat()
    {
        // End bar
        if (currentTime >= currentBarStartTime + currentBarDuration)
        {
            currentBarIndex++;
            currentBeatIndex = 0;
            if (hasSongEnded)
            {
                Debug.Log("Song ended!");
                OnSongEnded?.Invoke();
                return;
            }
            currentBarStartTime += currentBarDuration;
            Debug.Log($"Bar {currentBarIndex} started at {currentBarStartTime}");
        }

        var nextBeat = lastTriggerTime + beatDuration;
        if (currentTime >= nextBeat)
        {
            lastTriggerTime = nextBeat;
            Debug.Log($"Beat {(int)((currentTime - currentBarStartTime) / beatDuration)}: {currentTime}");
            OnBeatTrigger?.Invoke(lastTriggerTime, currentBeatIndex);
            currentBeatIndex++;
        }
    }

    public bool PlayNote(NoteType note)
    {
        if (!hasSongStarted || hasSongEnded) return false;

        var closestNoteKey = scoredBeats.Keys.Aggregate((curMin, x) => 
            MathF.Abs(x.Time - currentTime) < MathF.Abs(curMin.Time - currentTime) 
            ? x : curMin);

        if (MathF.Abs(closestNoteKey.Time - currentTime) < _hitThreshold)
        {
            if (scoredBeats.TryGetValue(closestNoteKey, out var scored) && !scored)
            {
                scoredBeats[closestNoteKey] = true;
                Debug.Log($"Hit! Note: {note}, Time: {currentTime}, Diff: {MathF.Abs(closestNoteKey.Time - currentTime)}");
                return true;
            }
            else
            {
                scoredBeats[new Note(){NoteType = note, Time = currentTime}] = false;
                Debug.Log($"Hit wrong note! Note: {note}, Time: {currentTime}");
            }
        }
        else
        {
            Debug.Log($"Missed! Note: {note}, Time: {currentTime}, Closest: {closestNoteKey.Time}");
        }

        return false;
    }

    public (int Score, int MaxScore) GetScore()
    {
        return (scoredBeats.Count(x => x.Value), scoredBeats.Count);
    }
}

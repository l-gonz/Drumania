using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SongController : MonoBehaviour
{
    [SerializeField] private int _countdown = 4;
    [SerializeField] private float _hitThreshold = .1f;
    [SerializeField] private SongData _currentSong;

    public event Action<int> OnCountdownTrigger;
    public event Action<float> OnBeatTrigger;

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

    private int currentTempo;
    private int currentBarIndex;
    private float currentTime;

    private float currentBarStartTime;

    private bool hasSongStarted;
    private float lastTriggerTime;

    private float beatDuration => 60f / currentTempo;
    private float currentBarDuration => song[currentBarIndex].BeatsPerBar * beatDuration;
    private bool hasSongEnded => currentBarIndex >= song.Count;

    private Dictionary<Beat, bool> scoredBeats = new Dictionary<Beat, bool>();
    private List<Bar> song;

    private void Start()
    {
        currentTempo = _currentSong.DefaultTempo;
        currentTime = - _countdown - 0.3f;

        var beatDuration = 60f / currentTempo;
        var accumulatedTime = 0f;
        foreach (var bar in _currentSong.Bars)
        {
            var barDuration = bar.BeatsPerBar  * beatDuration;
            foreach (var beat in bar.Beats)
            {
                scoredBeats.Add(beat with {Time = accumulatedTime + beat.Time * barDuration}, false);
            }
            accumulatedTime += barDuration;
        }

        // Add count in
        song = _currentSong.Bars.Prepend(new Bar()
        {
            BeatsPerBar = 4,
            BeatLength = .25f,
        }).ToList();
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
        }
    }

    private void HandleBeat()
    {
        // End bar
        if (currentTime >= currentBarStartTime + currentBarDuration)
        {
            currentBarIndex++;
            if (hasSongEnded)
            {
                Debug.Log("Song ended!");
                Debug.Log($"Score: {scoredBeats.Count(x => x.Value)} / {scoredBeats.Count}");
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
            OnBeatTrigger?.Invoke(lastTriggerTime);
        }
    }

    public bool PlayNote(Note note)
    {
        if (!hasSongStarted || hasSongEnded) return false;

        var closestNote = GetClosest32thNote(currentBarStartTime, currentBarStartTime + currentBarDuration, currentTime);
        if (MathF.Abs(closestNote - currentTime) < _hitThreshold)
        {
            var key = new Beat(){Note = note, Time = closestNote};
            if (scoredBeats.TryGetValue(key, out var scored) && !scored)
            {
                scoredBeats[key] = true;
                Debug.Log($"Hit! Note: {note}, Time: {currentTime}, Diff: {MathF.Abs(closestNote - currentTime)}");
                return true;
            }
            else
            {
                scoredBeats[new Beat(){Note = note, Time = currentTime}] = false;
                Debug.Log($"Hit wrong note! Note: {note}, Time: {currentTime}");
            }
        }
        return false;
    }

    private float GetClosest32thNote(float start, float end, float target)
    {
        int subdivisions = 32;
        float step = (end - start) / subdivisions;

        return Enumerable.Range(0, subdivisions)
                        .Select(i => new { Index = i, Point = start + i * step })
                        .OrderBy(x => MathF.Abs(x.Point - target))
                        .First().Point;
    }
}

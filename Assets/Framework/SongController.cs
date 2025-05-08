using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SongController : MonoBehaviour
{
    [SerializeField] private int _countdown = 4;
    [SerializeField] private float _hitThreshold = .1f;
    [SerializeField] private SongData _currentSong;

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
    private float currentBarDuration;

    private bool hasSongStarted;

    private Dictionary<Beat, bool> scoredBeats = new Dictionary<Beat, bool>();

    private void Start()
    {
        currentTempo = _currentSong.DefaultTempo;
        currentTime = - _countdown;

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

        currentBarDuration = _currentSong.Bars[currentBarIndex].BeatsPerBar * (60f / currentTempo);
    }

    private void Update()
    {
        if (currentBarIndex >= _currentSong.Bars.Length) return;

        if (MathF.Abs(currentTime - (int)currentTime) < 0.03f)
        {
            Debug.Log($"Current time: {(int)currentTime}");
        }

        currentTime += Time.deltaTime;
        if (!hasSongStarted && currentTime >= 0)
        {
            hasSongStarted = true;
            Debug.Log("Song started!");
        }

        if (hasSongStarted)
        {
            if (currentTime >= currentBarStartTime + currentBarDuration)
            {
                currentBarIndex++;
                currentBarStartTime = currentTime;
                if (currentBarIndex >= _currentSong.Bars.Length)
                {
                    Debug.Log("Song ended!");
                    Debug.Log($"Score: {scoredBeats.Count(x => x.Value)} / {scoredBeats.Count}");
                    return;
                }
                currentBarDuration = _currentSong.Bars[currentBarIndex].BeatsPerBar * (60f / currentTempo);
                Debug.Log($"Bar {currentBarIndex} started at {currentBarStartTime}");
            }
        }
    }

    public bool PlayNote(Note note)
    {
        if (!hasSongStarted || currentBarIndex >= _currentSong.Bars.Length) return false;

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

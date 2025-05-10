using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SongData", menuName = "Scriptable Objects/SongData")]
public class SongData : ScriptableObject
{
    public Bar[] Bars; // Array of bars in the song
    public int DefaultTempo; // Tempo of the song in beats per minute (BPM)
}

[Serializable]
public class Bar
{
    public int BeatsPerBar = 4; // Number of beats in the bar
    public float BeatLength = 1/4f; // Length of each beat
    public Beat[] Beats; // Array of beats in the bar
}

[Serializable]
public record Beat
{
    public Note Note;
    public float Time; // Position of the beat in the bar
}

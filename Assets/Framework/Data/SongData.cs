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
    public int Repeat = 1; // Number of times the bar is played
    public Note[] Notes; // Array of beats in the bar
}

[Serializable]
public record Note
{
    public NoteType NoteType;
    public float Time; // Position of the note in the bar
}

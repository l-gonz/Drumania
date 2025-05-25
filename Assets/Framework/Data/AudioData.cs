using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/AudioData")]
public class AudioData : ScriptableObject
{
    public AudioClipData[] AudioClips;

    public bool TryGetValue(AudioString id, out AudioClip clip)
    {
        try
        {
            clip = AudioClips.Single(clip => clip.ID == id)?.AudioClip;
        }
        catch (InvalidOperationException e)
        {
            Debug.LogWarning($"Error with audio clip with ID {id}: {e.Message}");
            clip = AudioClips.FirstOrDefault(clip => clip.ID == id)?.AudioClip;
        }
        return clip != null;
    }
}

[Serializable]
public class AudioClipData
{
    public AudioString ID;
    public AudioClip AudioClip;
}

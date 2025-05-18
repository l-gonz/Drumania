using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnareDrum : MonoBehaviour, IInstrument
{
    [SerializeField] private Animator _animator;
    [SerializeField] private TimingArtifact _timingPrefab;
    [SerializeField] private float _endPositionY = 0.35f;
    [SerializeField] private float _travelDistance = 2f;

    private const string GOOD_HIT_TRIGGER = "GoodHit";
    private const string BAD_HIT_TRIGGER = "BadHit";

    private List<Bar> song;
    private int currentTempo;
    private int nextBarIndex;
    
    
    private float beatDuration => 60f / currentTempo;
    private float barDuration => 60f / currentTempo * song[nextBarIndex].BeatsPerBar;
    public NoteType NoteType => NoteType.Snare;

    private void Start()
    {
        SongController.Instance.OnSongStarted += OnSongStarted;
        SongController.Instance.OnBeatTrigger += OnBeat;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayNote();
        }
    }

    public void PlayNote()
    {
        var isGoodHit = SongController.Instance.PlayNote(NoteType);
        _animator.SetTrigger(isGoodHit ? GOOD_HIT_TRIGGER : BAD_HIT_TRIGGER);
    }

    private void OnSongStarted(List<Bar> bars, int tempo)
    {
        song = bars;
        currentTempo = tempo;
    }

    private void OnBeat(float time, int beatIndex)
    {
        if (beatIndex == 0) nextBarIndex++;
        if (nextBarIndex >= song.Count) return;

        var nextBar = song[nextBarIndex];
        var notesInNextBeat = nextBar.Notes.Where(b => b.NoteType == NoteType 
                                                    && b.Time >= beatIndex * beatDuration 
                                                    && b.Time < (beatIndex + 1) * beatDuration);

        foreach (var note in notesInNextBeat)
        {
            var timingArtifact = Instantiate(_timingPrefab, 
                transform.position + Vector3.up * (_travelDistance + _endPositionY), 
                Quaternion.identity, transform);
            timingArtifact.SetSpeed(Vector3.down * _travelDistance / barDuration);
            timingArtifact.DelayedStart(note.Time * barDuration);
            timingArtifact.DelayedDestroy(barDuration * (1 + note.Time));
        }
    }
}

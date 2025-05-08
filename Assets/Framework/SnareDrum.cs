using UnityEngine;

public class SnareDrum : MonoBehaviour, IInstrument
{
    [SerializeField] private Animator _animator;

    private const string GOOD_HIT_TRIGGER = "GoodHit";
    private const string BAD_HIT_TRIGGER = "BadHit";
    
    public Note Note => Note.Snare;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayNote();
        }
    }

    public void PlayNote()
    {
        Debug.Log("Snare drum hit!");
        var isGoodHit = SongController.Instance.PlayNote(Note);
        _animator.SetTrigger(isGoodHit ? GOOD_HIT_TRIGGER : BAD_HIT_TRIGGER);
    }
}

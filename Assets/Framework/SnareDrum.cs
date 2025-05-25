using UnityEngine;

public class SnareDrum : Instrument
{
    public override NoteType NoteType => NoteType.Snare;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlayNote();
        }
    }

    protected override void PlayNote()
    {
        AudioManager.Instance.PlaySound(AudioString.Snare);
        base.PlayNote();
    }
}

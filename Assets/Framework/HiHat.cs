using UnityEngine;

public class HiHat : Instrument
{
    public override NoteType NoteType => NoteType.HiHat;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayNote();
        }
    }

    protected override void PlayNote()
    {
        AudioManager.Instance.PlaySound(AudioString.HiHat);
        base.PlayNote();
    }
}

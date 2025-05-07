using UnityEngine;

public class SnareDrum : MonoBehaviour, IInstrument
{
    public Note Note => Note.Snare; // The note associated with the snare drum

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Example: Play sound when space key is pressed
        {
            PlayNote();
        }
    }

    public void PlayNote()
    {
        Debug.Log("Snare drum hit!");
        SongController.Instance.PlayNote(Note);
    }
}

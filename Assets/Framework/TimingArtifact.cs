using UnityEngine;

public class TimingArtifact : MonoBehaviour
{
    [SerializeField] private Material _doneMaterial;

    private bool isStarted;

    private Vector3 speed;

    private void Update()
    {
        if (isStarted)
        {
            transform.position += speed * Time.deltaTime;
        }
    }

    public void SetSpeed(Vector3 newSpeed)
    {
        speed = newSpeed;
    }

    public void DelayedStart(float time)
    {
        Invoke(nameof(StartSelf), time);
    }

    public void DelayedDestroy(float time)
    {
        Invoke(nameof(MarkDestroy), time);
        Invoke(nameof(DestroySelf), time + 0.7f);
    }

    private void StartSelf()
    {
        gameObject.SetActive(true);
        isStarted = true;
    }

    private void MarkDestroy()
    {
        GetComponent<Renderer>().material = _doneMaterial;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
        isStarted = false;
    }
}

using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countdownText;

    public UIManager Instance { get; private set; }
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

    private void Start()
    {
        _countdownText.gameObject.SetActive(false);
        _countdownText.text = string.Empty;
        StartCountdown(4);
    }

    public void StartCountdown(int countdown)
    {
        StartCoroutine(CountdownRoutine(countdown));
    }

    private IEnumerator CountdownRoutine(int countdown)
    {
        _countdownText.gameObject.SetActive(true);
        for (int i = countdown - 1; i >= 0; i--)
        {
            if (i == 0)
            {
                _countdownText.text = "GO!";
            }
            else
            {
                _countdownText.text = i.ToString();
            }
            AudioManager.Instance.PlaySound("Countdown");
            yield return new WaitForSeconds(1f);
        }

        _countdownText.gameObject.SetActive(false);
    }
}

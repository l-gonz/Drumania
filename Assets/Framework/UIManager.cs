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
        SongController.Instance.OnCountdownTrigger += SetCountdown;
    }

    private void SetCountdown(int countdown)
    {
        _countdownText.gameObject.SetActive(true);
        if (countdown == 0)
        {
            _countdownText.text = "GO!";
            Invoke(nameof(HideCountdown), .5f);
        }
        else
        {
            _countdownText.text = countdown.ToString();
        }
    }

    private void HideCountdown()
    {
        _countdownText.gameObject.SetActive(false);
    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private Button _endGameButton;

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
        _endGameButton.gameObject.SetActive(false);

        _countdownText.text = string.Empty;
        _endGameButton.onClick.AddListener(EndGame);

        SongController.Instance.OnCountdownTrigger += SetCountdown;
        SongController.Instance.OnSongEnded += ShowScore;
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

    private void ShowScore()
    {
        _countdownText.gameObject.SetActive(true);
        var score = SongController.Instance.GetScore();
        _countdownText.text = $"Score: {score.Score} / {score.MaxScore}";
        _endGameButton.gameObject.SetActive(true);
    }

    private void EndGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

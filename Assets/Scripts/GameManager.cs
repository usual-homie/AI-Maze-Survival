using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text survivalTimeText;
    public TMP_Text bestTimeText;

    private float survivalTime;
    private bool gameOver;

    private const string BestTimeKey = "BestTime";

    public float SurvivalTime => survivalTime;
    public bool IsGameOver => gameOver;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Time.timeScale = 1f;
    }

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (gameOver)
            return;

        survivalTime += Time.deltaTime;
    }

    public void EndGame()
    {
        if (gameOver)
            return;

        gameOver = true;

        float bestTime = GetBestTime();

        if (survivalTime > bestTime)
        {
            PlayerPrefs.SetFloat(BestTimeKey, survivalTime);
            PlayerPrefs.Save();
            bestTime = survivalTime;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (survivalTimeText != null)
            survivalTimeText.text = "Survival Time : " + FormatTime(survivalTime);

        if (bestTimeText != null)
            bestTimeText.text = "Best Time : " + FormatTime(bestTime);

        Time.timeScale = 0f;
    }

    public float GetBestTime()
    {
        return PlayerPrefs.GetFloat(BestTimeKey, 0f);
    }

    public void ResetGame()
    {
        Time.timeScale = 1f;
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
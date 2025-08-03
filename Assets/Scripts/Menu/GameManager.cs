using UnityEngine;
using UnityEngine.SceneManagement;
using YG;  // PluginYourGames

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int Score      { get; private set; }
    public int HighScore  { get; private set; }

    [Header("Прокачка сложности")]
    [Tooltip("На сколько прибавлять moveSpeed в StackManager за каждый блок")]
    public float speedIncrement = 0.01f;

    private StackManager stackManager;
    private float initialSpeed;

    private const string LeaderboardId = "myLeaderboard";

    public GameObject resumePanel;

    void Awake()
    {
        // Синглтон
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Сохраняем стартовую скорость
        stackManager = FindObjectOfType<StackManager>();
        if (stackManager != null)
            initialSpeed = stackManager.moveSpeed;

        // Локальный запасной рекорд
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        Score     = 0;
    }

    void Start()
    {
        // Ждём инициализации SDK
        if (YG2.isSDKEnabled) InitCloud();
        else                 YG2.onGetSDKData += InitCloud;
    }

    void OnDestroy()
    {
        YG2.onGetSDKData -= InitCloud;
    }

    private void InitCloud()
    {
        // Storage-модуль автоматически загружает все поля в YG2.saves при старте :contentReference[oaicite:2]{index=2}
        HighScore = YG2.saves.highScore;
        Debug.Log($"[Cloud] Загружен highScore = {HighScore}");
    }

    /// <summary>Вызывать из StackManager при успешной установке блока</summary>
    public void OnBlockPlaced()
    {
        Score++;

        if (Score > HighScore)
        {
            HighScore = Score;
            YG2.saves.highScore = HighScore;
            YG2.SaveProgress();                  // сохраняем в облако и локально :contentReference[oaicite:3]{index=3}
            PlayerPrefs.SetInt("HighScore", HighScore);
        }

        if (stackManager != null)
            stackManager.moveSpeed += speedIncrement;
    }

    /// <summary>Game Over — пишем в лидерборд и показываем его UI</summary>
    public void GameOver()
    {
        // Запись в лидерборд :contentReference[oaicite:4]{index=4}
        YG2.SetLeaderboard(LeaderboardId, HighScore);

        // Если вы добавили в сцену префаб LeaderboardCanvas с компонентом LeaderboardYG,
        // то просто обновим его:
        var lb = FindObjectOfType<LeaderboardYG>();
        if (lb != null) lb.UpdateLB();

        //resumePanel.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        //resumePanel.transform.localPosition = Vector3.zero;
        Instantiate(resumePanel, FindObjectOfType<Canvas>().transform, false);

        //Restart();
    }

    /// <summary>Для совместимости с вашим FallingBlock.cs</summary>
    public void ResetGame()
    {
        GameOver();
    }

    public void Restart()
    {
        Score = 0;
        if (stackManager != null)
            stackManager.moveSpeed = initialSpeed;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

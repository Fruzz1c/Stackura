
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int Score { get; private set; }
    public int HighScore { get; private set; }

    [Header("Прокачка сложности")]
    [Tooltip("На сколько прибавлять moveSpeed в StackManager за каждый блок")]
    public float speedIncrement = 0.5f;

    private StackManager stackManager;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        // Загрузка рекорда
        HighScore = PlayerPrefs.GetInt("HighScore", 0);

        // найдём StackManager в сцене
        stackManager = FindObjectOfType<StackManager>();
    }

    /// <summary>
    /// Вызывается, когда игрок успешно поставил блок
    /// </summary>
    public void OnBlockPlaced()
    {
        // инкрементируем счёт
        Score++;

        // проверяем рекорд
        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt("HighScore", HighScore);
        }

        // повышаем сложность
        if (stackManager != null)
            stackManager.moveSpeed += speedIncrement;
    }

    /// <summary>
    /// Сброс игры (вызывать при Game Over)
    /// </summary>
    public void ResetGame()
    {
        Score = 0;
        // сброс скорости на первоначальную (если нужно)
        if (stackManager != null)
            stackManager.moveSpeed = stackManager.moveSpeed - Score * speedIncrement;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

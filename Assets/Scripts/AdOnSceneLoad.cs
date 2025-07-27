using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class AdOnSceneLoad : MonoBehaviour
{
    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;            // подписываемся на загрузку сцены
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        YG2.InterstitialAdvShow();                            // показываем рекламу
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;            // чистим подписку, когда объект умирает
    }
}
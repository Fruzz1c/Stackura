using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [Tooltip("UI-текст, в котором показываем счёт")]
    public TMP_Text scoreText;

    void Update()
    {
        if (GameManager.Instance != null)
            scoreText.text = $"{GameManager.Instance.Score}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonResume : MonoBehaviour
{
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        // Ищем объект GameManager в сцене
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager не найден!");
        }
    }

    // Метод, который будет вызываться при нажатии на кнопку
    public void OnButtonClick()
    {
        if (gameManager != null)
        {
            gameManager.Restart();
        }
    }
}

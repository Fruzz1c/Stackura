using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject baseBlock;
    public GameObject menuUI;
    public GameObject gameScore;

    public void startGame()
    {
        menuUI.SetActive(false);
        baseBlock.SetActive(true);
        gameScore.SetActive(true);
    }
}

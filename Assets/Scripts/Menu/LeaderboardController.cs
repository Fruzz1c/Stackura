using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    public GameObject leaderboardUI;
    public bool leaderboardIsOpen = false;

    public void ShowLeaderboard()
    {
        leaderboardIsOpen = !leaderboardIsOpen;
    }

    public void Update()
    {
        if (leaderboardIsOpen)
        {
            leaderboardUI.SetActive(true);
        }
        else
        {
            leaderboardUI.SetActive(false);
        }
    }
}

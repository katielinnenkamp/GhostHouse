using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager instance;

    public bool playingFullGame = false;
    public bool completedAnomalyGame = false;
    public bool hasOutsideKey = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartFullGame()
    {
        playingFullGame = true;
        completedAnomalyGame = false;
        hasOutsideKey = false;

        SceneManager.LoadScene("AlphaBuild");
    }

    public void PlayGhostHouseOnly()
    {
        playingFullGame = false;
        SceneManager.LoadScene("AlphaBuild");
    }

    public void PlayAnomalyGameOnly()
    {
        playingFullGame = false;
        SceneManager.LoadScene("anomaly");
    }

    public void PlayPlatformerGameOnly()
    {
        playingFullGame = false;
        SceneManager.LoadScene("parkour");
    }

    public void EnterAnomalyGame()
    {
        SceneManager.LoadScene("anomaly");
    }

    public void CompleteAnomalyGame()
    {
        completedAnomalyGame = true;
        SceneManager.LoadScene("AlphaBuild");
    }

    public void StartPlatformerGame()
    {
        SceneManager.LoadScene("parkour");
    }

    public void WinGame()
    {
        SceneManager.LoadScene("FullGameWin");
    }

    public void LoseGame()
    {
        SceneManager.LoadScene("FullGameLose");
    }

    public void ReturnToMainMenu()
    {
        playingFullGame = false;
        completedAnomalyGame = false;
        hasOutsideKey = false;

        SceneManager.LoadScene("BetaBuildMenu");
    }
}

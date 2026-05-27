using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenu;
    public playerMove player;

    private bool paused = false;

    void Start()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (paused)
        {
            ContinueGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        paused = true;

        if (player != null)
        {
            player.UnlockCursor();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ContinueGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        paused = false;

        if (player != null)
        {
            player.LockCursor();
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        paused = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("BetaBuildMenu");
    }
}

using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public void PlayFullGame()
    {
        GameFlowManager.instance.StartFullGame();
    }

    public void PlayGhostHouseOnly()
    {
        GameFlowManager.instance.PlayGhostHouseOnly();
    }

    public void PlayAnomalyGameOnly()
    {
        GameFlowManager.instance.PlayAnomalyGameOnly();
    }

    public void PlayPlatformerGameOnly()
    {
        GameFlowManager.instance.PlayPlatformerGameOnly();
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}

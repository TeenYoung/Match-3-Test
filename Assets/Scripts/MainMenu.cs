using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject menuPanel;

    public void MenuOnClick()
    {
        // Pause the game and show menu panel
        Time.timeScale = 0;
        menuPanel.SetActive(true);
    }

    public void ResumeOnClick()
    {
        // Disable menu panel and resume the game
        menuPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void RestartOnClick()
    {
        // Disable menu panel and restrat the game
        menuPanel.SetActive(false);
        BoardController.board.levelEndPanel.SetActive(false);
        Time.timeScale = 1;
        BoardController.board.Restart();
    }


}

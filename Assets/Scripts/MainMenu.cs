using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject bgmSwitch, soundFxSwitch;
    public Sprite bgmIcon, bgmMuteIcon, soundFxIcon, soundFxMuteIcon;

    private bool isBgmMute = false, isSoundFxMute =false;

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
        // Disable menu panel and level end panel then restrat the game
        menuPanel.SetActive(false);
        BoardController.board.levelEndPanel.SetActive(false);
        Time.timeScale = 1;
        BoardController.board.Restart();
    }

    public void BgmSwitchOnClick()
    {
        AudioController.audioController.bgmSource.mute = !AudioController.audioController.bgmSource.mute;
        isBgmMute = !isBgmMute;

        if (isBgmMute)
        {
            bgmSwitch.GetComponent<Image>().sprite = bgmMuteIcon;
        }
        else
        {
            bgmSwitch.GetComponent<Image>().sprite = bgmIcon;
        }
    }

    public void SoungFxSwitchOnClick()
    {
        AudioController.audioController.efxSource.mute = !AudioController.audioController.efxSource.mute;
        isSoundFxMute = !isSoundFxMute;

        if (isSoundFxMute)
        {
            soundFxSwitch.GetComponent<Image>().sprite = soundFxMuteIcon;
        }
        else
        {
            soundFxSwitch.GetComponent<Image>().sprite = soundFxIcon; 
        }
    }


}

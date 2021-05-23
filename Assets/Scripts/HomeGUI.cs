using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SWNetwork;

public class HomeGUI : MonoBehaviour
{
    public GameObject HomeMenu;
    public GameObject OptionsMenu;
    public GameObject SingleGameSetting;
    public GameObject AIPlayerSetting;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToSingleGameSetting()
    {
        SingleGameSetting.SetActive(true);
        HomeMenu.SetActive(false);
    }
    public void SingleGameSettingBack()
    {
        HomeMenu.SetActive(true);
        SingleGameSetting.SetActive(false);
    }
    public void ToSingleGameAISetting()
    {
        AIPlayerSetting.SetActive(true);
        SingleGameSetting.SetActive(false);
    }
    public void SingleGameAISettingBack()
    {
        AIPlayerSetting.SetActive(false);
        SingleGameSetting.SetActive(true);
    }
    public void SingleGameAISettingOK()
    {
        AIPlayerSetting.SetActive(false);
        SingleGameSetting.SetActive(true);
    }
    public void SingleGameStart()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void ToLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }
    public void ToOptions()
    {
        HomeMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }
    public void OptionsBack()
    {
        HomeMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }
    public void ExitGame()
    {
        Application.Quit();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SWNetwork;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /* --------- 联机向 ----------- */
    // 退出游戏，返回主界面
    public void Exit() 
    {
        SceneManager.LoadScene("HomeScene");
        NetworkClient.Lobby.LeaveRoom(HandleLeaveRoom);
    }
    void HandleLeaveRoom(bool okay, SWLobbyError error)
    {
        if (!okay)
        {
            Debug.LogError(error);
        }
        Debug.Log("Left room");
        SceneManager.LoadScene("HomeScene");
    }
    // 游戏场景初始化
    public void OnSpawnerReady(bool finishedSceneSetup, SceneSpawner sceneSpawner)
    {
        //Debug.Log("OnSpawnerReady " + finishedSceneSetup);
        if (!finishedSceneSetup)
        {
            int spawnPointIndex = Random.Range(0, 4);
            int playerPrefabIndex = Random.Range(0, 4);

            //sceneSpawner.SpawnForPlayer(spawnPointIndex, playerPrefabIndex);
            //sceneSpawner.SpawnForNonPlayer(0, 0);
      
            sceneSpawner.SpawnForPlayer(0, spawnPointIndex);
          
            sceneSpawner.PlayerFinishedSceneSetup();
            Debug.Log("OnSpawnerReady " + finishedSceneSetup);
        }
    }
    public void OnHostSpawnerReady(bool alreadySetup, SceneSpawner sceneSpawner) 
    {
        Debug.Log("OnHostSpawnerReady " + alreadySetup);
        if (!alreadySetup)
        {
            sceneSpawner.HostFinishedSceneSetup();
        }
    }
}
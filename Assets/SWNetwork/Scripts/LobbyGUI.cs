using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SWNetwork;

public class LobbyGUI : MonoBehaviour // lobbyGUI class to control the interactions between UI (show or hide Popup panel)
{
    public Lobby lobby; // lobby instance to control lobby events

    /*----UI of lobby----*/
    public GameObject roomRowPrefab; // 
    public GameObject roomList;

    public GameObject playerRowPrefab;
    public GameObject teamHeaderRowPrefab;
    public GameObject playerList;

    public GameObject messageRowPrefab;
    public GameObject messageList;

    public Text LobbyPing; // display ping in game lobby
    public Text playerNameText; // player name display in lobby
    public InputField playerIdText; //input field for enter player id to send private message
    public InputField messagePlayerText; //input field for enter message to send privately
    public InputField messageRoomText; // input field for enter room message
    public InputField newRoomText; // input field for enter new room name

    public GameObject newRoomPopup; //create new room panel
    public GameObject CreatingRoomMessagePopup; //creating new room panel
    public GameObject messagePlayerPopup; //send message to selected player panel
    public GameObject sendRoomMessageErrorPopup;//error notice panel when fail in sending room message
    public GameObject CreateRoomErrorPopup;//error notice panel when fail in creating room 
    public GameObject LeaveRoomErrorPopup;//error notice panel when fail in leaving room 
    public GameObject JoinRoomErrorPopup;//error notice panel when fail in joining room message
    public GameObject BackToEntryPopup; //notice panel when player try to leave the lobby and back to lobby entry

    // The current message row count.
    int currentMessageRowCount = 0;
    int MAX_MESSAGE_ROW_COUNT = 5;

    //Callbak to invoke when NewGamePopup is closed.
    Action<bool, string> newGamePopupCloseCallback;

    // Callback to invoke when MessagePlayerPopup is closed.
    Action<bool, string, string> messagePlayerPopupCloseCallback;

    // Add a row to the Player list.
    public void AddRowForPlayer(string title, string objectId, TableRow.SelectedHandler callback)
    {
        AddRowToTable(playerList.transform, playerRowPrefab, title, objectId, callback);
    }

    // Add a row to the Team list.
    public void AddRowForTeam(string title) 
    {
        AddRowToTable(playerList.transform, teamHeaderRowPrefab, title, null, null);
    }

    // Add a row to the Room list.
    public void AddRowForRoom(string title, string objectId, TableRow.SelectedHandler callback)
    {
        AddRowToTable(roomList.transform, roomRowPrefab, title, objectId, callback);
    }

    // Add a row to the Message list
    public void AddRowForMessage(string title, string objectId, TableRow.SelectedHandler callback)
    {
        if (currentMessageRowCount == MAX_MESSAGE_ROW_COUNT)
        {
            //remove the first message when MAX_MESSAGE_ROW_COUNT is reached.
            RemoveChild(messageList.transform);
            currentMessageRowCount--;
        }
        currentMessageRowCount++;
        AddRowToTable(messageList.transform, messageRowPrefab, title, objectId, callback);
        messageRoomText.text = "";
    }
    void AddRowToTable(Transform table, GameObject rowPrefab, string title, string objectId, TableRow.SelectedHandler callback)
    {
        GameObject newRow = Instantiate(rowPrefab, table);
        TableRow tableRow = newRow.GetComponent<TableRow>();
        tableRow.OnSelected += callback;
        tableRow.SetTitle(title);
        tableRow.SetObjectId(objectId);
    }

    // Remove all the rows in the Player list.
    public void ClearPlayerList()
    {
        RemoveAllChildren(playerList.transform);
    }
    // Remove all the rows in the Room list.
    public void ClearRoomList()
    {
        RemoveAllChildren(roomList.transform);
    }
    // Remove all the messages in room chat.
    public void ClearRoomMessage()
    {
        Destroy(messageList.gameObject);
        currentMessageRowCount = 0;
    }
    void RemoveAllChildren(Transform parent)
    {
        foreach (Transform childTransform in parent)
        {
            Destroy(childTransform.gameObject);
        }
    }
    void RemoveChild(Transform parent)
    {
        foreach (Transform childTransform in parent)
        {
            Destroy(childTransform.gameObject);
            return;
        }
    }

    /*----Methods to control popup panels----*/
    public void ShowBackToEntryPopup()
    {
        BackToEntryPopup.SetActive(true);
    }
    public void ShowNewRoomPopup(Action<bool, string> callback)
    {
        newRoomPopup.SetActive(true);
        newRoomText.text = "";
        newGamePopupCloseCallback = callback;
    }
    public void ShowMessagePlayerPopup(string targetPlayer, Action<bool, string, string> callback)
    {
        messagePlayerPopup.SetActive(true);
        playerIdText.text = targetPlayer;
        messagePlayerText.text = "";
        messagePlayerPopupCloseCallback = callback;
    }
    public void ShowCreateRoomErrorPopup()
    {
        CreateRoomErrorPopup.SetActive(true);
    }
    public void ShowJoinRoomErrorPopup()
    {
        JoinRoomErrorPopup.SetActive(true);
    }
    public void ShowLeaveRoomErrorPopup()
    {
        LeaveRoomErrorPopup.SetActive(true);
    }
    public void ShowSendRoomMessageErrorPopup()
    {
        sendRoomMessageErrorPopup.SetActive(true);
    }

    /*----Methods to deal with the buttons in popup panels----*/
    public void HandleBackToEntryOk()
    {
        NetworkClient.Lobby.LeaveRoom((successful, error) =>
        {
            if (successful)
            {
                Debug.Log("Left room");
                ClearPlayerList();
                ClearRoomMessage();
            }
            else
            {
                Debug.Log("Failed to leave room " + error);
            }
        });
        SceneManager.LoadScene("LobbyScene");
    }
    public void HandleBackToEntryCancel()
    {
        BackToEntryPopup.SetActive(false);
    }
    public void HandleCreateRoomOK()
    {
        string roomName = newRoomText.text;
        lobby.ExecutionFailed.SetActive(false);
        lobby.ExecutionPassed.SetActive(false);
        if (roomName.Length == 0)
        {
            lobby.AutoExecutionResultText.text += "Room name can't be empty\r\n";
            lobby.ExecutionFailed.SetActive(true);
        }
        if (roomName.Length > lobby.MaxCharOfRoomName)
        {
            lobby.AutoExecutionResultText.text += "Room name must not longer than " + lobby.MaxCharOfRoomName + " character\r\n";
            lobby.ExecutionFailed.SetActive(true);
        }
        string spe = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~ ";
        int speCount = 0;
        for (int i = 0; i < roomName.Length; i++)
        {
            if (spe.Contains(roomName[i].ToString()))
            {
                lobby.AutoExecutionResultText.text += "Illegal character '" + roomName[i] + "' detected\r\n";
                lobby.ExecutionFailed.SetActive(true);
                speCount++;
            }
        }
        if (roomName.Length > 0 && roomName.Length <= lobby.MaxCharOfRoomName && speCount==0)
        {
            CreatingRoomMessagePopup.SetActive(true);
            newRoomPopup.SetActive(false);
            lobby.AutoExecutionResultText.text += "Room name verified\r\n";
            if (newGamePopupCloseCallback != null)
            {
                newGamePopupCloseCallback(true, roomName);
            }
        }
    }
    public void HandleCreateRoomCancel()
    {
        newRoomPopup.SetActive(false);
        if (newGamePopupCloseCallback != null)
        {
            newGamePopupCloseCallback(false, null);
        }
    }
    public void HandleMessagePlayerOK()
    {
        if (playerIdText.text.Length > 0)
        {
            messagePlayerPopup.SetActive(false);
            if (messagePlayerPopupCloseCallback != null)
            {
                messagePlayerPopupCloseCallback(true, playerIdText.text, messagePlayerText.text);
            }
        }
        else
        {
            Debug.LogWarning("Player id is empty.");
        }
    }
    public void HandleMessagePlayerCancel()
    {
        messagePlayerPopup.SetActive(false);
        if (messagePlayerPopupCloseCallback != null)
        {
            messagePlayerPopupCloseCallback(false, null, null);
        }
    }
    public void HandleCreateRoomErrorOk()
    {
        CreateRoomErrorPopup.SetActive(false);
    }
    public void HandleJoinRoomErrorOk()
    {
        JoinRoomErrorPopup.SetActive(false);
    }
    public void HandleLeaveRoomErrorOk()
    {
        LeaveRoomErrorPopup.SetActive(false);
    }
    public void HandleSendRoomMessageErrorOk()
    {
        sendRoomMessageErrorPopup.SetActive(false);
    }
}
